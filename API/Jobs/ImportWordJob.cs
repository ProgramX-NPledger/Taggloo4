﻿using System.Collections.ObjectModel;
using API.Contract;
using API.Model;
using Hangfire;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;

namespace API.Jobs;

public class ImportWordJob
{
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	public ImportWordJob()
	{
		
	}
	
	public ImportWordJob(IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository)
	{
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
	}

	/// <summary>
	/// Imports a Word and performs required live-indexing.
	/// </summary>
	/// <param name="remoteHostAddress">The host address of the user importing the Word.</param>
	/// <param name="currentUserName">The username od the user importing the Word.</param>
	/// <param name="timeStamp">The date/time the Word was created. If not specified, use current date/time.</param>
	/// <param name="theWord">The Word being imported.</param>
	/// <param name="dictionaryId">The ID of the Dictionary that will own the Word.</param>
	/// <param name="importGuid">A unique identifier generated by the importing endpoint to allow asynchronous correlation.</param>
	/// <exception cref="ImportException">Thrown if the import fails.</exception>
	//[Queue("import")]
    public void ImportWord(string remoteHostAddress,
	    string? currentUserName,
	    string theWord,
	    int dictionaryId, 
	    Guid importGuid,
	    DateTime? timeStamp=null)
    {
	    Dictionary? dictionary = _dictionaryRepository.GetByIdAsync(dictionaryId).Result;
	    if (dictionary == null) throw new ImportException($"Dictionary ID {dictionaryId} not found");

	    Word word = CreateNewWordOrGetExistingWord(theWord, dictionaryId, remoteHostAddress, currentUserName, importGuid,timeStamp ?? DateTime.Now);

	    // link with existing phrases
		IEnumerable<Phrase> phrasesWithText= _phraseRepository.GetPhrasesAsync(null, null, theWord).Result;
		foreach (Phrase phrase in phrasesWithText)
		{
			// must be a complete word and not be a part of another word
			bool wordIsADiscreteWord=IsWordADiscreteWordWithinPhrase(theWord, phrase.ThePhrase);
			if (wordIsADiscreteWord && phrase.Dictionary!=null)
			{
				// does phrase dictionary language match the word language?
				if (phrase.Dictionary.IetfLanguageTag == dictionary.IetfLanguageTag && word.Phrases!=null)
				{
					word.Phrases.Add(phrase);
					_wordRepository.Update(word);
				}
			}
		}

		_wordRepository.SaveAllAsync();
		
    }

	private Word CreateNewWordOrGetExistingWord(string word, int dictionaryId, string remoteHostAddress,
		string? currentUserName, Guid importGuid, DateTime timeStamp)
	{
		IEnumerable<Word> existingWords = _wordRepository.GetWordsAsync(word, dictionaryId).Result.ToArray();
		if (existingWords.Count() > 1)
			throw new ImportException(
				$"Attempt to find existing Word '{word}' in Dictionary ID {dictionaryId} resulted in {existingWords.Count()} results, expected 0 or 1");
		if (existingWords.Any())
		{
			return existingWords.Single();
		}
		else
		{
			Word newWord = new Word()
			{
				CreatedAt = timeStamp,
				CreatedOn = remoteHostAddress,
				CreatedByUserName = currentUserName,
				TheWord = word,
				DictionaryId = dictionaryId,
				ImportId = importGuid,
				Phrases = new Collection<Phrase>()
			};

			_wordRepository.Create(newWord);
			if (!_wordRepository.SaveAllAsync().Result)
				throw new ImportException(newWord.TheWord, "Word");

			return newWord;
		}
	}

	private bool IsWordADiscreteWordWithinPhrase(string word, string phrase)
    {
	    string lowerPhrase = phrase.ToLower();
	    string lowerWord = word.ToLower();
	    return lowerPhrase == lowerWord ||
	           lowerPhrase.Contains(lowerWord + " ") ||
	           lowerPhrase.Contains(" " + lowerWord);
    }
    
}