using API.Contract;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

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