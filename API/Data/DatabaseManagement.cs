using API.Contract;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DatabaseManagement : IDatabaseManagement
{
    private readonly DataContext _dataContext;

    public DatabaseManagement(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
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