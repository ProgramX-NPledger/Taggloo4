using System.ComponentModel.DataAnnotations;

namespace API.Model;

/// <summary>
/// A logged API event.
/// </summary>
public class ApiLog
{
	/// <summary>
	/// Unique identifier for the API Log event.
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// IP Address of requester.
	/// </summary>
	public string? IpAddress { get; set; }
	
	/// <summary>
	/// Timestamp of request.
	/// </summary>
	[DataType(DataType.DateTime)]
	public DateTime TimeStamp { get; set; }
	
	/// <summary>
	/// The HTTP Verb used in the request.
	/// </summary>
	[MaxLength(12)]
	public string? RequestVerb { get; set; }
	
	/// <summary>
	/// The safe URL (having been sanitised of potentially secret information)
	/// </summary>
	[MaxLength(2048)]
	public string? SafeUrl { get; set; }
	
	/// <summary>
	/// The HTTP response code.
	/// </summary>
	public int ResponseCode { get; set; }
	
	/// <summary>
	/// The text response.
	/// </summary>
	public string? ResponseText { get; set; }
	
	/// <summary>
	/// Length of time taken for the request to be actioned in milliseconds (ms).
	/// </summary>
	public double TimeMs { get; set; }
	
	
}