using System.ComponentModel.DataAnnotations;

namespace API.Model;

public class ApiLog
{
	public int Id { get; set; }
	public string IpAddress { get; set; }
	[DataType(DataType.DateTime)]
	public DateTime TimeStamp { get; set; }
	[MaxLength(12)]
	public string RequestVerb { get; set; }
	[MaxLength(2048)]
	public string SafeUrl { get; set; }
	public int ResponseCode { get; set; }
	public string? ResponseText { get; set; }
	public double TimeMs { get; set; }
	
	
}