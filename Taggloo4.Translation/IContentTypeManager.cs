﻿using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Model;

namespace Taggloo4.Translation;

public interface IContentTypeManager
{
    IContentTypeManager Initialise(DataContext dataContext, Dictionary dictionary);
    Task DeleteDictionaryAndContentsAsync();
    Task<int> GetNumberOfItemsAsync();
}