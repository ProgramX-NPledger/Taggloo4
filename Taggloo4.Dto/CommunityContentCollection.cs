using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a collection of <seealso cref="CommunityContentItem"/>s.
/// </summary>
public class CommunityContentCollection
{
	/// <summary>
    /// Identifier of Community Content Collection.
    /// </summary>
    public int Id { get; set; }

	/// <summary>
	/// Name of Collection.
	/// </summary>
	[MaxLength(128)]
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// URL to retrieve items in Collection.
	/// </summary>
	[MaxLength(256)]
	[JsonPropertyName("searchUrl")]
	public string? SearchUrl { get; set; }

	/// <summary>
	/// Discoverer capable of retrieving Content.
	/// </summary>
	[JsonPropertyName("discoverer")]
	public CommunityContentDiscoverer? Discoverer { get; set; }

	/// <summary>
	/// If set, required poll frequency in minutes.
	/// </summary>
	[JsonPropertyName("pollFrequencyMins")]
	public int? PollFrequencyMins { get; set; }
	
	/// <summary>
	/// If specified, the time the Collection was last polled for new content.
	/// </summary>
	[JsonPropertyName("lastPolledAt")] 
	public DateTime? LastPolledAt { get; set; }

	/// <summary>
	/// If set, enables automatic polling.
	/// </summary>
	[JsonPropertyName("isPollingEnabled")]
	public bool? IsPollingEnabled { get; set; }
	
	
}