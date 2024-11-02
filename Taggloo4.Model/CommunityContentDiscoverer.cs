using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Taggloo4.Model;

public class CommunityContentDiscoverer
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public required string CommunityContentDiscovererDotNetAssemblyName { get; set; }

    [MaxLength(256)]
    public required string CommunityContentDiscovererDotNetTypeName { get; set; }

    public Collection<CommunityContentCollection>? CommunityContentCollections { get; set; }
}