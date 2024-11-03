using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

/// <summary>
/// A collection of Community Content Items.
/// </summary>
public class CommunityContentCollection
{
    /// <summary>
    /// Identifier of collection.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of collection.
    /// </summary>
    [MaxLength(128)]
    public required string Name { get; set; }
    
    /// <summary>
    /// URL to use when searching for content.
    /// </summary>
    [MaxLength(256)]
    public string? SearchUrl { get; set; }
    
    /// <summary>
    /// Identifier of Discoverer used to retrieve content.
    /// </summary>
    public int CommunityContentDiscovererId { get; set; }

    /// <summary>
    /// Navigational property of Discoverer used to retrieve content.
    /// </summary>
    public CommunityContentDiscoverer? CommunityContentDiscoverer { get; set; }
    
    /// <summary>
    /// Frequency of polling (minutes).
    /// </summary>
    public int? PollFrequencyMins { get; set; }
    
    /// <summary>
    /// Timestamp of last poll.
    /// </summary>
    public DateTime? LastPolledAt { get; set; }
    
    /// <summary>
    /// Whether automatic polling is enabled.
    /// </summary>
    public bool IsPollingEnabled { get; set; }
    
    /// <summary>
    /// Navigational property for Community Content Items in collection.
    /// </summary>
    public Collection<CommunityContentItem>? CommunityContentItems { get; set; } = [];

}