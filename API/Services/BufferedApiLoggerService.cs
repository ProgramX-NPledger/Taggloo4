using API.Contract;
using API.Model;
using Hangfire;

namespace API.Services;

/// <summary>
/// Implementation of <seealso cref="IApiLoggerService"/> that uses Hangfire for buffered writing of API activity.
/// </summary>
public class BufferedApiLoggerService : IApiLoggerService
{
	private readonly IApiLogRepository _apiLogRepository;
	private static List<ApiLog> _apiLog= new();
	
	/// <summary>
	/// Constructor with injected parameter.
	/// </summary>
	/// <param name="apiLogRepository">The <seealso cref="IApiLogRepository"/> to use when persisting to the data store.</param>
	public BufferedApiLoggerService(IApiLogRepository apiLogRepository)
	{
		_apiLogRepository = apiLogRepository;
		
		
		RecurringJob.AddOrUpdate("bufferedApiLogging",() => WriteApiLogBuffer(), Cron.Minutely);
	}

	/// <summary>
	/// Adds a logged API call to the buffer for delayed writing.
	/// </summary>
	/// <returns><c>Task.CompletedTask</c> to indicate completion.</returns>
	public Task WriteApiLogBuffer()
	{
		lock (_apiLog)
		{
			_apiLogRepository.SaveAll(_apiLog);
			_apiLog.Clear();
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Adds a log to the API Log buffer.
	/// </summary>
	/// <param name="ipAddress">IP Address of request.</param>
	/// <param name="requestVerb">HTTP method/vern of request.</param>
	/// <param name="safeUrl">Sanitised URL for storage (ensure there are no secrets!)</param>
	/// <param name="responseCode">Response HTTP code</param>
	/// <param name="responseText">Response HTTP text</param>
	/// <param name="timeMs">Time taken to perform the request.</param>
	public void Log(string ipAddress, string requestVerb, string safeUrl, int responseCode, string responseText, double timeMs)
	{
		_apiLog.Add(new ApiLog()
		{
			IpAddress = ipAddress,
			RequestVerb = requestVerb,
			ResponseCode = responseCode,
			ResponseText = responseText,
			SafeUrl = safeUrl,
			TimeMs = timeMs,
			TimeStamp = DateTime.Now
		});
	}
}