using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Taggloo4.Dto;

/// <summary>
/// Represents a result within <seealso cref="GetCommunityContentCollectionResult"/>.
/// </summary>
public class GetCommunityContentCollectionResultItem : CommunityContentCollection
{
	/// <summary>
	/// Links associated with this request.
	/// </summary>
	public IEnumerable<Link>? Links { get; set; }
}