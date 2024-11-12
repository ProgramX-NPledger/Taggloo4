using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Contract.Criteria;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;
using Taggloo4.Model.Exceptions;

namespace Taggloo4.Repository;

/// <summary>
/// Represents a repository for working with Dictionaries.
/// </summary>
public class ContentTypeRepository : RepositoryBase<ContentType>, IContentTypeRepository
{
	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public ContentTypeRepository(DataContext dataContext) : base(dataContext)
	{
	}
	


	/// <inheritdoc cref="GetContentTypeAsync"/>
	public async Task<ContentType?> GetContentTypeAsync(string contentTypeKey)
	{
		return await DataContext.ContentTypes
			.Include(m=>m.Dictionaries).ThenInclude(m=>m.Language)
			.SingleOrDefaultAsync(q => q.ContentTypeKey == contentTypeKey);
	}

	/// <inheritdoc cref="GetContentTypesAsync"/>
	public async Task<IEnumerable<ContentType>> GetContentTypesAsync(string? contentTypeKey, string? nameSingular, string? namePlural,
		string? contentTypeManagerDotNetAssemblyName, string? contentTypeManagerDotNetTypeName)
	{
		IQueryable<ContentType> query = DataContext.ContentTypes.AsQueryable();
		if (!string.IsNullOrWhiteSpace(contentTypeKey))
		{
			query = query.Where(q => q.ContentTypeKey == contentTypeKey);
		}

		if (!string.IsNullOrWhiteSpace(nameSingular))
		{
			query = query.Where(q => q.NameSingular == nameSingular);
		}
		
		if (!string.IsNullOrWhiteSpace(namePlural))
		{
			query = query.Where(q => q.NamePlural == namePlural);
		}
		
		if (!string.IsNullOrWhiteSpace(contentTypeManagerDotNetAssemblyName))
		{
			query = query.Where(q => q.ContentTypeManagerDotNetAssemblyName == contentTypeManagerDotNetAssemblyName);
		}

		if (!string.IsNullOrWhiteSpace(contentTypeManagerDotNetTypeName))
		{
			query = query.Where(q => q.ContentTypeManagerDotNetTypeName == contentTypeManagerDotNetTypeName);
		}
		
		return await query.ToArrayAsync();	
	}
}