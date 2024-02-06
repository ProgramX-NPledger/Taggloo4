namespace API.Middleware.Model;

public class HttpRequestForLog
{
	public string IpAddress { get; set; }
	public DateTime TimeStamp { get; set; }
	public string RequestVerb { get; set; }
	public string SafeUrl { get; set; }
	
}