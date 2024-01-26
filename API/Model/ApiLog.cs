namespace API.Model;

public class ApiLog
{
	public int Id { get; set; }
	public string IpAddress { get; set; }
	public DateTime TimeStamp { get; set; }
	public string RequestVerb { get; set; }
	public string SafeUrl { get; set; }
	public int ResponseCode { get; set; }
	public string? ResponseText { get; set; }
	public double TimeMs { get; set; }
	
	
}