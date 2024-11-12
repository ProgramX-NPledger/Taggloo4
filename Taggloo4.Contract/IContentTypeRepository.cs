using Taggloo4.Contract.Criteria;
using Taggloo4.Model;
using Taggloo4.Model.Exceptions;

namespace Taggloo4.Contract;

/// <summary>
/// Represents an abstraction for working with Dictionaries.
/// </summary>
public interface IContentTypeRepository : IRepositoryBase<ContentType>
{

	/// <summary>
	/// Return the requested Content Type.
	/// </summary>
	/// <param name="contentTypeKey">Identifier of Content Type.</param>
	/// <returns>The requested <seealso cref="ContentType"/>, or <c>null</c> if not found.</returns>
	Task<ContentType?> GetContentTypeAsync(string contentTypeKey);
	
	/// <summary>
	/// Return a collection of matching Content Types.
	/// </summary>
	/// <param name="contentTypeKey">If specified, the identifier of the Content type.</param>
	/// <param name="nameSingular">If specified, the name of the content (singular).</param>
	/// <param name="namePlural">If specified, the name of the content (plural).</param>
	/// <param name="contentTypeManagerDotNetAssemblyName">If specified, the .NET Assembly of the Content Type Manager.</param>
	/// <param name="contentTypeManagerDotNetTypeName">If specified, the .NET Type name of the Content Type Manager.</param>
	/// <returns>A colleciton of matching <seealso cref="ContentType"/>s.</returns>
	Task<IEnumerable<ContentType>> GetContentTypesAsync(string? contentTypeKey, string? nameSingular, string? namePlural, string? contentTypeManagerDotNetAssemblyName, string? contentTypeManagerDotNetTypeName);
	
	
}