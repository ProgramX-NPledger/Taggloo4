using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with API Logs.
/// </summary>
public interface IApiLogRepository
{
	
	
	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <param name="apiLogs">Collection of <seealso cref="ApiLog"/> objects to write.</param>
	/// <returns>Number of affected rows.</returns>
	int SaveAll(IEnumerable<ApiLog> apiLogs);
}