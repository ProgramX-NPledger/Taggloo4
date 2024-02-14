namespace API.Middleware.Model;

public class HttpRequestForLog
{
	public required string IpAddress { get; set; }
	public DateTime TimeStamp { get; set; }
	public required string RequestVerb { get; set; }
	public required string SafeUrl { get; set; }
	
}