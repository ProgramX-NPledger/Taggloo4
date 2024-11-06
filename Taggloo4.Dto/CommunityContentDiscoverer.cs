using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a Discoverer capable of retrieving Community Content.
/// </summary>
public class CommunityContentDiscoverer
{
    /// <summary>
    /// Identifier of Discoverer
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Name of Discoverer.
    /// </summary>
    [MaxLength(128)]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// .NET Assembly Name containing type capable of discovering new content.
    /// </summary>
    [MaxLength(256)]
    [JsonPropertyName("communityContentDiscovererDotNetAssemblyName")]
    public string? CommunityContentDiscovererDotNetAssemblyName { get; set; }
    
    /// <summary>
    /// .NET Type name capable of discovering new content.
    /// </summary>
    [MaxLength(256)]
    [JsonPropertyName("communityContentDiscovererDotNetTypeName")]
    public string? CommunityContentDiscovererDotNetTypeName { get; set; }

    /// <summary>
    /// Key of Discoverer.
    /// </summary>
    [MaxLength(128)]
    [JsonPropertyName("key")]
    public required string Key { get; set; }
}