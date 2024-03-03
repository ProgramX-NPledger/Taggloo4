using API.Contract;
using API.Model;

namespace API.Jobs;

public class EnforceDatabaseSizeJob
{
    public const string MAX_DATABASE_SIZE_BYTES_CONFIG_KEY = "Database:SizeManagement:MaximumSizeMb";
    
    private readonly IDatabaseManagement _databaseManagement;
    private readonly IConfiguration _configuration;

    public EnforceDatabaseSizeJob(IDatabaseManagement databaseManagement, IConfiguration configuration)
    {
        _databaseManagement = databaseManagement;
        _configuration = configuration;
    }

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
            int recordsAffected=_databaseManagement.DeleteOldestLogsByDay();

            if (recordsAffected < 100)
            {
                // gone too far, likely will not be able to reocver any more space
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