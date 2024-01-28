namespace API.Contract;

/// <summary>
/// Provides API Logging services.
/// </summary>
public interface IApiLoggerService
{
	/// <summary>
	/// Adds a log to the API Log buffer.
	/// </summary>
	/// <param name="ipAddress">IP Address of request.</param>
	/// <param name="requestVerb">HTTP method/vern of request.</param>
	/// <param name="safeUrl">Sanitised URL for storage (ensure there are no secrets!)</param>
	/// <param name="responseCode">Response HTTP code</param>
	/// <param name="responseText">Response HTTP text</param>
	/// <param name="timeMs">Time taken to perform the request.</param>
	void Log(string ipAddress, string requestVerb, string safeUrl, int responseCode, string responseText, double timeMs);
}