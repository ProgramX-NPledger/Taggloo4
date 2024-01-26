namespace API.Contract;

public interface IApiLogRepository
{
	Task Log(string ipAddress, string requestVerb, string safeUrl, int responseCode, string responseText, double timeMs);
}