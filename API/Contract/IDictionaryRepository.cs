﻿using API.Model;

namespace API.Contract;

/// <summary>
/// Represents an abstraction for working with Dictionaries.
/// </summary>
public interface IDictionaryRepository
{
	/// <summary>
	/// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="dictionary">Entity to mark as updated.</param>
	void Update(Dictionary dictionary);
	
	/// <summary>
	/// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
	/// </summary>
	/// <param name="dictionary">Entity to create.</param>
	void Create(Dictionary dictionary);
	
	/// <summary>
	/// Saves changes to the data store.
	/// </summary>
	/// <returns><c>True</c> if successful.</returns>
	Task<bool> SaveAllAsync();

	/// <summary>
	/// Retrieves a <seealso cref="Dictionary"/> by its ID.
	/// </summary>
	/// <param name="id">The ID of the <seealso cref="Dictionary"/>.</param>
	/// <returns>The requested <seealso cref="Dictionary"/>, or <c>null</c> if no Dictionary could be found./</returns>
	Task<Dictionary?> GetById(int id);
	

}