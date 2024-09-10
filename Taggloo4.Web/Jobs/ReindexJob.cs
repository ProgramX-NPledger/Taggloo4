using System.Collections.ObjectModel;
using Hangfire;
using Taggloo4.Contract;
using Taggloo4.Dto;
using Taggloo4.Model;
using Taggloo4.Model.Exceptions;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Jobs;

/// <summary>
/// Job to allow reindexing of database items.
/// </summary>
public class ReindexJob
{
	private readonly ILanguageRepository _languageRepository;
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly IWordInPhraseRepository _wordInPhraseRepository;
	private readonly IDatabaseManagement _databaseManagement;
	private readonly ILogger<ReindexJob> _logger;


	/// <summary>
	/// Reindexes the database for performant querying.
	/// </summary>
	/// <param name="languageRepository">Implementation of <see cref="ILanguageRepository"/>.</param>
	/// <param name="wordRepository">Implementation of <see cref="IWordRepository"/>.</param>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="wordInPhraseRepository">Implementation of <see cref="IWordInPhraseRepository"/>.</param>
	/// <param name="databaseManagement">Implementation of <see cref="IDatabaseManagement"/>.</param>
	/// <param name="logger"><see cref="ILogger{T}"/> for logging.</param>
	public ReindexJob(ILanguageRepository languageRepository,
		IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository,
		IWordInPhraseRepository wordInPhraseRepository,
		IDatabaseManagement databaseManagement,
		ILogger<ReindexJob> logger)
	{
		this._languageRepository = languageRepository;
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		_wordInPhraseRepository = wordInPhraseRepository;
		_databaseManagement = databaseManagement;
		_logger = logger;
		
	}

	/// <summary>
	/// Executes re-indexing of the database.
	/// </summary>
	/// <remarks>This method is not async because Hangfire does not support asynchronous tasks.</remarks>
	public void Reindex()
	{
		// make sure there is not an indexing job currently running
		IEnumerable<ReindexingJob> activeReindexingJobs = _databaseManagement.GetReindexingJobsAsync(true).Result;
		if (activeReindexingJobs.Any())
		{
			_logger.LogError(
				$"Cannot execute multiple reindexing jobs concurrently. Currently executing IDs: {string.Join(", ", activeReindexingJobs.Select(q => q.Id).ToArray())}");
			return;
		}

		int reindexingJobId =
			_databaseManagement.StartReindexingJobAsync($"{Environment.UserDomainName}\\{Environment.UserName}",
				Environment.MachineName).Result;
		
		_logger.LogInformation("Reindexing (Reindexing Job ID: {reindexingJobId})",reindexingJobId);
		
		int numberOfLanguagesProcessed = 0;
		int numberOfDictionariesProcessed = 0;
		int numberOfPhrasesProcessed = 0;
		int numberOfWordsProcessed = 0;
		int numberOfWordsCreated = 0;
		int numberOfWordInPhrasesCreated = 0;
		int numberOfWordInPhrasesRemoved = 0;
		
		ReindexWordsInPhrases(reindexingJobId,
			ref numberOfLanguagesProcessed,
			ref numberOfDictionariesProcessed,
			ref numberOfPhrasesProcessed,
			ref numberOfWordsCreated,
			ref numberOfWordsCreated,
			ref numberOfWordInPhrasesCreated,
			ref numberOfWordInPhrasesRemoved);
	
		_databaseManagement.CompleteReindexingJobAsync(reindexingJobId,
			numberOfLanguagesProcessed,
			numberOfDictionariesProcessed,
			numberOfPhrasesProcessed,
			numberOfWordsProcessed,
			numberOfWordsCreated,
			numberOfDictionariesProcessed,
			numberOfWordInPhrasesRemoved
		);
    }

	private void ReindexWordsInPhrases(int reindexingJobId,
		ref int numberOfLanguagesProcessed,
		ref int numberOfDictionariesProcessed,
		ref int numberOfPhrasesProcessed,
		ref int numberOfWordsProcessed,
		ref int numberOfWordsCreated,
		ref int numberOfWordInPhrasesCreated,
		ref int numberOfWordInPhrasesRemoved)
	{
	
	    
	    _logger.LogInformation("Reindexing Words in Phrases (Correlation ID: {reindexingJobId})",reindexingJobId);
	    
	    // for each language
	    IEnumerable<Language> allLanguages = _languageRepository.GetAllLanguagesAsync().Result;

	    foreach (Language language in allLanguages)
	    {
		    _logger.LogInformation("Reindexing Words in Phrases for Language {languageCode} (Correlation ID: {reindexingJobId}",language.IetfLanguageTag,reindexingJobId);
		    
		    IEnumerable<Dictionary> allDictionariesInLanguage = _dictionaryRepository.GetDictionariesAsync(null, language.IetfLanguageTag).Result;
		    foreach (Dictionary dictionary in allDictionariesInLanguage)
		    {
			    _logger.LogInformation("Reindexing Words in Phrases for Dictionary ID {dictionaryId} for Language {languageCode} (Correlation ID: {reindexingJobId})",dictionary.Id,language.IetfLanguageTag,reindexingJobId);
			    
			    IEnumerable<Phrase> allPhrasesInDictionary = _phraseRepository.GetPhrasesAsync(null, dictionary.Id, null, null, null).Result.ToArray();
				_logger.LogInformation("Dictionary ID {dictionaryId} has {phrasesCount} phrases for reindexing (Correlation ID: {reindexingJobId})",dictionary.Id, allPhrasesInDictionary.Count(), reindexingJobId);

				foreach (Phrase phrase in allPhrasesInDictionary)
			    {
				    // break apart phrase into words
				    string[] words = phrase.ThePhrase
					    .Split(new char[] { ' ' })
					    .Select(q => q.Trim())
					    .Where(q=>!string.IsNullOrWhiteSpace(q))
					    .ToArray();

				    int ordinal = 0;
				    foreach (string wordString in words)
				    {
					    ordinal++;
					    
					    IList<Word> matchingWords = _wordRepository.GetWordsAsync(wordString, dictionary.Id, null).Result.ToList();
					    if (!matchingWords.Any())
					    {
						    // no matching words, so create it and add it to the collection
						    _logger.LogInformation(
							    "Creating new Word \"{word}\" in Dictionary ID {dictionaryId} because it isn't already present (Correlation ID: {reindexingJobId})",
							    wordString, dictionary.Id, reindexingJobId);
						    
						    // BUG: This is attempting to create multiple Words within the same Dictionary
						    // also: https://learn.microsoft.com/en-gb/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues
						    // use separate DbContext instances
						    Word newWord = new Word()
						    {
							    CreatedAt = DateTime.Now,
							    CreatedOn =Environment.MachineName,
							    TheWord = wordString,
							    CreatedByUserName = $"{Environment.UserDomainName}\\{Environment.UserName}",
							    Dictionary = dictionary
						    };
						    _wordRepository.Create(newWord);
						    _wordRepository.SaveAllAsync().Wait();
						    matchingWords.Add(newWord);

						    numberOfWordsCreated++;
					    }
					    
					    foreach (Word word in matchingWords)
					    {
						    // there should only be one word in the collection, but if there are >1, they are all relevant
										
						    // is there already a WordInPhrase entry?
						    IEnumerable<WordInPhrase> wordInPhrases = _wordRepository.GetPhrasesForWordAsync(word.Id,phrase.Id,ordinal).Result;

						    if (!wordInPhrases.Any())
						    {
							    _logger.LogInformation("Creating a new WordInPhrase index because Word ID {wordId}/Phrase ID {phraseId} at ordinal {ordinal} has not yet been indexed (Correlation ID: {reindexingJobId})",
								    word.Id,phrase.Id,ordinal,reindexingJobId);
							    
							    // if not, create
							    WordInPhrase newWordInPhrase = new WordInPhrase()
							    {
								    CreatedOn = Environment.MachineName,
								    CreatedByUserName = $"{Environment.UserDomainName}\\{Environment.UserName}",
								    CreatedAt = DateTime.Now,
								    Ordinal = ordinal,
								    Word = word,
								    InPhrase = phrase
							    };
							    _wordRepository.AddPhraseForWord(newWordInPhrase);
							    _wordRepository.SaveAllAsync().Wait();

							    numberOfWordInPhrasesCreated++;
						    }

						    numberOfWordsProcessed++;
					    } // foreach Word
				    } // foreach wordString

				    numberOfPhrasesProcessed++;
			    }// foreach Phrase

			    numberOfDictionariesProcessed++;
		    }// foreach Dictionary

		    numberOfLanguagesProcessed++;
	    }// foreach Language

	    _logger.LogInformation("Completed building index (Correlation ID {reindexingJobId})", reindexingJobId);
	    
	    IEnumerable<WordInPhrase> wordsInPhrases = _wordInPhraseRepository.GetWordsInPhrasesAsync(null,null).Result;
	    foreach (WordInPhrase wordInPhrase in wordsInPhrases)
	    {
		    int recordsAffected = 0;
		    
		    Word? word = _wordRepository.GetByIdAsync(wordInPhrase.WordId).Result;
		    if (word == null)
		    {
			    _logger.LogInformation("Removing WordInPhrase ID {wordInPhraseId} because Word ID {wordId} no longer exists (Correlation ID: {reindexingJobId})",
				    wordInPhrase.Id,wordInPhrase.WordId,reindexingJobId);

			    _wordInPhraseRepository.Remove(wordInPhrase);

			    recordsAffected++;
		    }

		    Phrase? phrase = _phraseRepository.GetByIdAsync(wordInPhrase.InPhraseId).Result;
		    if (phrase == null)
		    {
			    _logger.LogInformation("Removing WordInPhrase ID {wordInPhraseId} because Phrase ID {phraseId} no longer exists (Correlation ID: {reindexingJobId})",
				    wordInPhrase.Id,wordInPhrase.InPhraseId,reindexingJobId);
			    
			    _wordInPhraseRepository.Remove(wordInPhrase);
			    recordsAffected++;
		    }
		    
		    // if word is no longer within phrase, remove WordInPhrase
		    if (phrase != null && word != null)
		    {
			    // make sure the word is a discrete word and not part of another word
			    if (IsWordADiscreteWordWithinPhrase(word.TheWord, phrase.ThePhrase))
			    {
				    _logger.LogInformation("Removing WordInPhrase ID {wordInPhraseId} because Word \"{word}\" is no longer in the phrase \"{phrase}\" (Correlation ID: {reindexingJobId})",
					    wordInPhrase.Id,word.TheWord,phrase.ThePhrase,reindexingJobId);
				    
				    _wordInPhraseRepository.Remove(wordInPhrase);
				    recordsAffected++;
			    }
		    }
			
		    if (recordsAffected > 0)
		    {
			    _wordInPhraseRepository.SaveAllAsync().Wait();

			    numberOfWordInPhrasesRemoved += recordsAffected;
		    }
	    }

	    _logger.LogInformation("Completed cleaning orphaned WordInPhrases (Correlation ID: {reindexingJobId})",reindexingJobId);
	    

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