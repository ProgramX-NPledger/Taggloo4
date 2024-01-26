using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApiLogRepository : IApiLogRepository
{
	private readonly DataContext _dataContext;

	public ApiLogRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	
	
	public async Task Log(string ipAddress, string requestVerb, string safeUrl, int responseCode, string responseText, double timeMs)
	{
		_dataContext.ApiLogs.Add(new ApiLog()
		{
			ResponseCode = responseCode,
			IpAddress = ipAddress,
			RequestVerb = requestVerb,
			ResponseText = responseText,
			SafeUrl = safeUrl,
			TimeStamp = DateTime.Now,
			TimeMs = timeMs
		});
		await _dataContext.SaveChangesAsync();
	}
}