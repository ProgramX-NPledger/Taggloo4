using API.Contract;
using API.Data.Migrations;
using API.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with API Logs.
/// </summary>
public class ApiLogRepository : IApiLogRepository
{
	private readonly string _connectionString;
	

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="configuration">Application configuration.</param>
	public ApiLogRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("DefaultConnection");
		if (_connectionString == null) throw new NullReferenceException("DefaultConnection string not configured");
		
	}

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public int SaveAll(IEnumerable<ApiLog> apiLogs)
	{
		int recordsAffected = 0;
		foreach (ApiLog apiLog in apiLogs.ToArray())
		{
			recordsAffected+=WriteApiLogToDatabase(apiLog);
		}

		return recordsAffected;
	}

	private int WriteApiLogToDatabase(ApiLog apiLog)
	{
		string sqlCmd = @"INSERT INTO [ApiLogs]
           ([IpAddress]
           ,[TimeStamp]
           ,[RequestVerb]
           ,[SafeUrl]
           ,[ResponseCode]
           ,[ResponseText]
           ,[TimeMs])
     VALUES
           (@IpAddress
           ,@TimeStamp
           ,@RequestVerb
           ,@SafeUrl
           ,@ResponseCode
           ,@ResponseText
           ,@TimeMs)";
		using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
		{
			sqlConnection.Open();
			using (SqlCommand sqlCommand = new SqlCommand(sqlCmd, sqlConnection))
			{
				sqlCommand.Parameters.AddWithValue("@IpAddress", apiLog.IpAddress);
				sqlCommand.Parameters.AddWithValue("@TimeStamp", apiLog.TimeStamp);
				sqlCommand.Parameters.AddWithValue("@RequestVerb", apiLog.RequestVerb);
				sqlCommand.Parameters.AddWithValue("@SafeUrl", apiLog.SafeUrl);
				sqlCommand.Parameters.AddWithValue("@ResponseCode", apiLog.ResponseCode);
				sqlCommand.Parameters.AddWithValue("@ResponseText", apiLog.ResponseText);
				sqlCommand.Parameters.AddWithValue("@TimeMs", apiLog.TimeMs);
				return sqlCommand.ExecuteNonQuery();
			}
		}
	}
}