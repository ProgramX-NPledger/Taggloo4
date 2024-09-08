using Taggloo4.Contract;
using Taggloo4.Web.Model;
using Taggloo4.Web.Contract;

namespace Taggloo4.Web.Jobs;

/// <summary>
/// Job to enforce required database size.
/// </summary>
/// <remarks>
/// SQL Server Express is restricted to &lt; 10 Gb, this job will enforce this restriction.
/// </remarks>
public class EnforceDatabaseSizeJob
{
    /// <summary>
    /// Configuration key identifying permitted database size.
    /// </summary>
    public const string MAX_DATABASE_SIZE_BYTES_CONFIG_KEY = "Database:SizeManagement:MaximumSizeMb";
    
    private readonly IDatabaseManagement _databaseManagement;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor with injected parameters.
    /// </summary>
    /// <param name="databaseManagement">Implementation of <see cref="IDatabaseManagement"/>.</param>
    /// <param name="configuration">Implementation of <see cref="IConfiguration"/>.</param>
    public EnforceDatabaseSizeJob(IDatabaseManagement databaseManagement, IConfiguration configuration)
    {
        _databaseManagement = databaseManagement;
        _configuration = configuration;
    }

    /// <summary>
    /// Enforce database size according to configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the processing failed.</exception>
    public void EnforceDatabaseSize()
    {
        long maximumDatabaseSize = _configuration.GetValue<long>(MAX_DATABASE_SIZE_BYTES_CONFIG_KEY);
        
        // get database size
        decimal dbSize = _databaseManagement.GetDatabaseSize();
        int iterations = 0;
        while (dbSize > maximumDatabaseSize)
        {
            iterations++;
            
            // delete old logs

            int recordsAffected = 0;
            recordsAffected+=_databaseManagement.DeleteOldestLogsByDay();
            recordsAffected += _databaseManagement.DeleteLogsByPropertiesText(new[]
            {
                "<property key='commandType'",
                "<property key='EndpointName'",
                "<property key='ObjectResultType'",
                "<property key='EventId'><structure type=''><property key='Id'>1</property></structure></property>",
                "<property key='RouteData'>"
                
            });
            
            if (iterations>10 && recordsAffected == 0)
            {
                // gone too far, likely will not be able to recover any more space
                throw new InvalidOperationException(
                    $"Failed to recover free space to return to maximum configured permitted size of {maximumDatabaseSize} Mb in database across {iterations} iterations from current database size of {dbSize} Mb");
            }
            
            // shrink
            _databaseManagement.ShrinkDatabase();
        
            // get size again
            dbSize = _databaseManagement.GetDatabaseSize();
            
        }
    }
}