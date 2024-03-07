using System.Collections.ObjectModel;
using API.Contract;
using API.Model;
using Hangfire;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;

namespace API.Jobs;

public class ImportPhraseJob
{
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	public ImportPhraseJob(IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository)
	{
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
	}
    
	/// <summary>
	/// Imports a Phrase and performs required live-indexing.
	/// </summary>
	/// <param name="remoteHostAddress">The host address of the user importing the Word.</param>
	/// <param name="currentUserName">The username od the user importing the Word.</param>
	/// <param name="theWord">The Word being imported.</param>
	/// <param name="dictionaryId">The ID of the Dictionary that will own the Word.</param>
	/// <exception cref="ImportException">Thrown if the import fails.</exception>
	[Queue("import")]
    public void ImportPhrase(string remoteHostAddress, 
	    string? currentUserName, 
	    string thePhrase, 
	    int dictionaryId,
	    Guid importGuid)
    {
	    Dictionary? dictionary = _dictionaryRepository.GetByIdAsync(dictionaryId).Result;
	    if (dictionary == null) throw new ImportException($"Dictionary ID {dictionaryId} not found");

	    Phrase phrase =
		    CreateNewPhraseOrGetExistingPhrase(thePhrase, dictionaryId, remoteHostAddress, currentUserName, importGuid);
	    
		string[] wordsInPhrase = thePhrase.Split(new char[] { ' ' });
		foreach (string wordInPhrase in wordsInPhrase)
		{
			IEnumerable<Word> existingWordsInPhrase = _wordRepository.GetWordsAsync(wordInPhrase, null).Result;
			bool foundWord = false;
			foreach (Word existingWordInPhrase in existingWordsInPhrase)
			{
				// dictionary must be in same language 
				if (existingWordInPhrase.Dictionary!=null &&
				    existingWordInPhrase.Dictionary.IetfLanguageTag == dictionary.IetfLanguageTag &&
				    existingWordInPhrase.Phrases!=null)
				{
					foundWord = true;
					// link the word with the phrase
					existingWordInPhrase.Phrases.Add(phrase);
					_wordRepository.Update(existingWordInPhrase);
					_wordRepository.SaveAllAsync();
				}
			}

			if (!foundWord)
			{
				// create word
				Word newWord = new Word()
				{
					CreatedAt = DateTime.Now,
					CreatedOn = remoteHostAddress,
					CreatedByUserName = currentUserName,
					TheWord = wordInPhrase,
					DictionaryId = dictionary.Id, // bug: will create word link in same dictionary as phrase, should get Dictionary in same language with Words - cannot select dictionary because do not know purpose of dictionary without putting a type on it
					Phrases = new Collection<Phrase>(
					[
						phrase
					])
				};
				_wordRepository.Create(newWord);
				_wordRepository.SaveAllAsync();
			}
			

			
		}
		
    }

	private Phrase CreateNewPhraseOrGetExistingPhrase(string phrase, int dictionaryId, string remoteHostAddress, string? currentUserName, Guid importGuid)
	{
		IEnumerable<Phrase> existingPhrases = _phraseRepository.GetPhrasesAsync(phrase, dictionaryId, null).Result.ToArray();
		if (existingPhrases.Count() > 1)
			throw new ImportException(
				$"Attempt to find existing Phrase '{phrase}' in Dictionary ID {dictionaryId} resulted in {existingPhrases.Count()} results, expected 0 or 1");
		if (existingPhrases.Any())
		{
			return existingPhrases.Single();
		}
		else
		{
			Phrase newPhrase = new Phrase()
			{
				CreatedAt = DateTime.Now,
				CreatedOn = remoteHostAddress,
				CreatedByUserName = currentUserName,
				ThePhrase = phrase,
				DictionaryId = dictionaryId,
				ImportId = importGuid,
				Words = new Collection<Word>()
			};

			_phraseRepository.Create(newPhrase);
			if (!_phraseRepository.SaveAllAsync().Result)
				throw new ImportException(newPhrase.ThePhrase, "Phrase");

			return newPhrase;
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