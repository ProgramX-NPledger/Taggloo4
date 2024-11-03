using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

/// <summary>
/// A Discoverer points to the code that retrieves content from the internet.
/// </summary>
public class CommunityContentDiscoverer
{
    /// <summary>
    /// Identifier of Discoverer.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of Discoverer.
    /// </summary>
    [MaxLength(128)]
    public required string Name { get; set; }

    /// <summary>
    /// Unique key to identify Discoverer.
    /// </summary>
    [MaxLength(128)]
    public required string Key { get; set; }
    
    /// <summary>
    /// Description of the Discoverer.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// .NET Assembly Name of Discoverer. If not defined, discovery is implicitly disabled.
    /// </summary>
    [MaxLength(256)]
    public string? CommunityContentDiscovererDotNetAssemblyName { get; set; }

    /// <summary>
    /// .NET Type Name of Discoverer. if not defined, discovery is implicitly disabled.
    /// </summary>
    [MaxLength(256)]
    public string? CommunityContentDiscovererDotNetTypeName { get; set; }

    /// <summary>
    /// Navigational property for Community Content Collections that use this Discoverer.
    /// </summary>
    public Collection<CommunityContentCollection>? CommunityContentCollections { get; set; }
}