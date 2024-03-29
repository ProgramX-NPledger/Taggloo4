﻿using API.Contract;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Represents a repository for working with Dictionaries.
/// </summary>
public class DictionaryRepository : IDictionaryRepository
{
	private readonly DataContext _dataContext;

	/// <summary>
	/// Constructor with injected Entity Framework <seealso cref="DataContext"/>.
	/// </summary>
	/// <param name="dataContext">Entity Framework <seealso cref="DataContext"/>.</param>
	public DictionaryRepository(DataContext dataContext)
	{
		_dataContext = dataContext;
	}

	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="dictionary">Entity to create.</param>
	public void Create(Dictionary dictionary)
	{
		_dataContext.Dictionaries.Add(dictionary);
	}
	
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="dictionary">Entity to mark as updated.</param>
	public void Update(Dictionary dictionary)
	{
		_dataContext.Entry(dictionary).State = EntityState.Modified;
	}

	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	public async Task<bool> SaveAllAsync()
	{
		return await _dataContext.SaveChangesAsync() > 0;
	}

	/// <summary>
	/// Retrieves a <seealso cref="Dictionary"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Dictionary"/>.</param>
	/// <returns>The requested <seealso cref="Dictionary"/>, or <c>null</c> if no Dictionary could be found./</returns>
	public async Task<Dictionary?> GetByIdAsync(int id)
	{
		return await _dataContext.Dictionaries.SingleOrDefaultAsync(q => q.Id == id);
	}

	/// <summary>
	/// Retrieves all matching <seealso cref="Dictionary"/> items.
	/// </summary>
	/// <param name="id">If specified, limits by the ID of the Dictionary.</param>
	/// <param name="ietfLanguageTag">If specified, limits by the IETF Language Tag for the Dictionary.</param>
	/// <returns>A collection of matching <seealso cref="Dictionary"/> items.</returns>
	public async Task<IEnumerable<Dictionary>> GetDictionariesAsync(int? id, string? ietfLanguageTag)
	{
		IQueryable<Dictionary> query = _dataContext.Dictionaries.AsQueryable();
		if (id.HasValue)
		{
			query = query.Where(q => q.Id == id.Value);
		}

		if (!string.IsNullOrWhiteSpace(ietfLanguageTag))
		{
			query = query.Where(q => q.IetfLanguageTag == ietfLanguageTag);
		}

		return await query.ToArrayAsync();
	}

	/// <summary>
	/// Deletes the specified Dictionary, and all related content.
	/// </summary>
	/// <param name="dictionaryId">ID of Dictionary to delete.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if deletion fails.</exception>
	public async Task Delete(int dictionaryId)
	{
		Dictionary dictionary = _dataContext.Dictionaries.Single(q => q.Id == dictionaryId);
		_dataContext.Dictionaries.Remove(dictionary);
		await _dataContext.SaveChangesAsync();
	}
}