using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Model;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Data;

/// <summary>
/// Implementation for database maintenance tasks.
/// </summary>
public class DatabaseManagement : IDatabaseManagement
{
    private readonly DataContext _dataContext;

    /// <summary>
    /// COnstructor with injected parameters.
    /// </summary>
    /// <param name="dataContext">The Entity Framework <c>DataContext</c>.</param>
    public DatabaseManagement(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    /// <summary>
    /// Returns the database size in Megabytes.
    /// </summary>
    /// <returns>The database size in Megabytes.</returns>
    public decimal GetDatabaseSize()
    {
        using (SqlConnection sqlConnection = new SqlConnection(_dataContext.Database.GetConnectionString()))
        {
            sqlConnection.Open();

            string sqlCmd = @"EXEC sp_spaceused;";
            using (SqlCommand sqlCommand = new SqlCommand(sqlCmd, sqlConnection))
            {
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    if (sqlDataReader.Read())
                    {
                        string databaseSizeMbAsString = sqlDataReader.GetString(1); // 3184.25 MB
                        string cleanedDatabaseSizeMbAsString = new string(
                            databaseSizeMbAsString.Where(c => "0123456789.".Contains(c)).ToArray());
                        
                        decimal databaseSizeMb;
                        if (decimal.TryParse(cleanedDatabaseSizeMbAsString, out databaseSizeMb))
                            return databaseSizeMb;
                        throw new InvalidOperationException($"Unable to convert database size form string into long");
                    }
                }
            }
        }

        throw new InvalidOperationException("Unable to get database size");
    }
    
    /// <summary>
    /// Deletes old logs.
    /// </summary>
    /// <returns>Number of records deleted.</returns>
    public int DeleteOldestLogsByDay()
    {
        using (SqlConnection sqlConnection = new SqlConnection(_dataContext.Database.GetConnectionString()))
        {
            sqlConnection.Open();

            string sqlCmd = @"delete from Serilog where convert(date,[timestamp])=(select min(convert(date,[timestamp])) from SeriLog);";
            using (SqlCommand sqlCommand = new SqlCommand(sqlCmd, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0; // disable timeouts
                int rowsAffected=sqlCommand.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }

    /// <summary>
    /// Deletes logs by text strings.
    /// </summary>
    /// <param name="textStrings">A string-array containing text strings that may appear in log messages that should be deleted.</param>
    /// <returns>Number of records deleted.</returns>
    public int DeleteLogsByPropertiesText(IEnumerable<string> textStrings)
    {
        using (SqlConnection sqlConnection = new SqlConnection(_dataContext.Database.GetConnectionString()))
        {
            sqlConnection.Open();
            int recordsAffected = 0;
            
            foreach (string textString in textStrings)
            {
                string sqlCmd = @"delete from Serilog where properties like @t";
                using (SqlCommand sqlCommand = new SqlCommand(sqlCmd, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0; // disable timeouts
                    sqlCommand.Parameters.AddWithValue("@t", $"%{textString}%");
                    recordsAffected+=sqlCommand.ExecuteNonQuery();
                    
                }
                
            }

            return recordsAffected;
        }
    }

    /// <summary>
    /// Retrieves Reindexing Jobs.
    /// </summary>
    /// <param name="isActive">When specified, filters by status of execution status.</param>
    /// <returns>A collection of <see cref="ReindexingJob"/> items.</returns>
    public async Task<IEnumerable<ReindexingJob>> GetReindexingJobsAsync(bool? isActive)
    {
        IQueryable<ReindexingJob> query = _dataContext.ReindexingJobs;

        if (isActive.HasValue)
        {
            query = query.Where(q => !q.FinishedAt.HasValue);
        }

        query = query.OrderBy(q => q.StartedAt);
        return await query.ToArrayAsync();
    }

    /// <summary>
    /// Marks a re-indexing job as being started.
    /// </summary>
    /// <param name="userName">Username of starting user.</param>
    /// <param name="hostName">Name of machine on which job was started.</param>
    /// <returns>The identifier of the reindexing job.</returns>
    public async Task<int> StartReindexingJobAsync(string userName, string hostName)
    {
        ReindexingJob newReindexingJob = new ReindexingJob()
        {
            StartedOn = hostName,
            StartedByUserName = userName,
            StartedAt = DateTime.Now
        };
        _dataContext.ReindexingJobs.Add(newReindexingJob);
        int recordsAffected=await _dataContext.SaveChangesAsync();
        if (recordsAffected > 0)
        {
            return newReindexingJob.Id;
        }

        throw new InvalidOperationException(
            $"Expected 1 record to be updated when marking Reindexing Job as being started but none were");
    }

    /// <summary>
    /// Marks a re-indexing job as being completed.
    /// </summary>
    /// <param name="reindexingJobId">The identifier of the job to complete. This is provided by the <see cref="StartReindexingJobAsync"/> function.</param>
    /// <param name="numberOfLanguagesProcessed">Number of <see cref="Language"/>s processed.</param>
    /// <param name="numberOfDictionariesProcessed">Number of <see cref="Dictionary"/> items processed.</param>
    /// <param name="numberOfPhrasesProcessed">Number of <see cref="Phrase"/>s processed.</param>
    /// <param name="numberOfWordsProcessed">Number of <see cref="Word"/>s processed.</param>
    /// <param name="numberOfWordsCreated">Number of <see cref="Word"/>s that were created.</param>
    /// <param name="numberOfWordsInPhrasesCreated">Number of <see cref="WordInPhrase"/> items created.</param>
    /// <param name="numberOfWordsInPhrasesRemoved">Number of <see cref="WordInPhrase"/> items removed due to removal of links.</param>
    /// <returns><c>True</c> if successful.</returns>
    public async Task<bool> CompleteReindexingJobAsync(int reindexingJobId, int numberOfLanguagesProcessed, int numberOfDictionariesProcessed,
        int numberOfPhrasesProcessed, int numberOfWordsProcessed, int numberOfWordsCreated,
        int numberOfWordsInPhrasesCreated, int numberOfWordsInPhrasesRemoved)
    {
        ReindexingJob? reindexingJob =
            await _dataContext.ReindexingJobs.SingleOrDefaultAsync(q => q.Id == reindexingJobId);
        if (reindexingJob == null)
        {
            throw new InvalidOperationException(
                $"Cannot mark non-existent Reindexing Job (ID: {reindexingJobId}) as Complete");
        }
        
        reindexingJob.FinishedAt=DateTime.Now;
        reindexingJob.NumberOfLanguagesProcessed = numberOfLanguagesProcessed;
        reindexingJob.NumberOfDictionariesProcessed = numberOfDictionariesProcessed;
        reindexingJob.NumberOfPhrasesProcessed = numberOfPhrasesProcessed;
        reindexingJob.NumberOfWordProcessed = numberOfWordsProcessed;
        reindexingJob.NumberOfWordsCreated = numberOfWordsCreated;
        reindexingJob.NumberOfWordsInPhrasesCreated = numberOfWordsInPhrasesCreated;
        reindexingJob.NumberOfWordsInPhrasesRemoved = numberOfWordsInPhrasesRemoved;
        _dataContext.ReindexingJobs.Update(reindexingJob);
        int recordsAffected = await _dataContext.SaveChangesAsync();
        return recordsAffected > 0;
    }

    /// <summary>
    /// Shrinks the database using known tooling of the DBMS.
    /// </summary>
    public void ShrinkDatabase()
    {
        using (SqlConnection sqlConnection = new SqlConnection(_dataContext.Database.GetConnectionString()))
        {
             sqlConnection.Open();

             string sqlCmd = @"DBCC SHRINKDATABASE(N'Taggloo4' );";
             using (SqlCommand sqlCommand = new SqlCommand(sqlCmd, sqlConnection))
             {
                 sqlCommand.ExecuteNonQuery();
             }
        }
    }
}