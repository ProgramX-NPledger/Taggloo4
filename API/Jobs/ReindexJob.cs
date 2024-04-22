using System.Collections.ObjectModel;
using API.Contract;
using API.Model;
using Hangfire;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;

namespace API.Jobs;

public class ReindexJob
{
	private readonly ILanguageRepository _languageRepository;
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;
	private readonly IWordInPhraseRepository _wordInPhraseRepository;


	/// <summary>
	/// Reindexes the database for performant querying.
	/// </summary>
	/// <param name="languageRepository">Implementation of <see cref="ILanguageRepository"/>.</param>
	/// <param name="wordRepository">Implementation of <see cref="IWordRepository"/>.</param>
	/// <param name="phraseRepository">Implementation of <see cref="IPhraseRepository"/>.</param>
	/// <param name="dictionaryRepository">Implementation of <see cref="IDictionaryRepository"/>.</param>
	/// <param name="wordInPhraseRepository">Implementation of <see cref="IWordInPhraseRepository"/>.</param>
	public ReindexJob(ILanguageRepository languageRepository,
		IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository,
		IWordInPhraseRepository wordInPhraseRepository)
	{
		this._languageRepository = languageRepository;
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		_wordInPhraseRepository = wordInPhraseRepository;
	}

	/// <summary>
	/// Executes re-indexing of the database.
	/// </summary>
	/// <remarks>This method is not async because Hangfire does not support asynchronous tasks.</remarks>
    public void Reindex()
    {
	    // for each language
	    IEnumerable<Language> allLanguages = _languageRepository.GetAllLanguagesAsync().Result;

	    foreach (Language language in allLanguages)
	    {
		    IEnumerable<Dictionary> allDictionariesInLanguage = _dictionaryRepository.GetDictionariesAsync(null, language.IetfLanguageTag).Result;

		    foreach (Dictionary dictionary in allDictionariesInLanguage)
		    {
			    IEnumerable<Phrase> allPhrasesInDictionary = _phraseRepository.GetPhrasesAsync(null, dictionary.Id, null, null, null).Result;

			    foreach (Phrase phrase in allPhrasesInDictionary)
			    {
				    // break apart phrase into words
				    string[] words = phrase.ThePhrase.Split(new char[] { ' ' }).Select(q => q.Trim()).ToArray();

				    int ordinal = 0;
				    foreach (string wordString in words)
				    {
					    ordinal++;
					    
					    IList<Word> matchingWords = _wordRepository.GetWordsAsync(wordString, dictionary.Id, null).Result.ToList();
					    if (!matchingWords.Any())
					    {
						    // no matching words, so create it and add it to the collection
						    // BUG: This is attempting to create multiple Words within the same Dictionary
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
					    }
					    
					    foreach (Word word in matchingWords)
					    {
						    // there should only be one word in the collection, but if there are >1, they are all relevant
										
						    // is there already a WordInPhrase entry?
						    IEnumerable<WordInPhrase> wordInPhrases = _wordRepository.GetPhrasesForWordAsync(word.Id,phrase.Id).Result;

						    if (!wordInPhrases.Any())
						    {
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
						    }
					    } // foreach Word
				    } // foreach wordString
			    }// foreach Phrase
		    }// foreach Dictionary
	    }// foreach Language

	    IEnumerable<WordInPhrase> wordsInPhrases = _wordInPhraseRepository.GetWordsInPhrasesAsync().Result;
	    foreach (WordInPhrase wordInPhrase in wordsInPhrases)
	    {
		    int recordsAffected = 0;
		    
		    Word? word = _wordRepository.GetByIdAsync(wordInPhrase.WordId).Result;
		    if (word == null)
		    {
			    _wordInPhraseRepository.Remove(wordInPhrase);
			    recordsAffected++;
		    }

		    Phrase? phrase = _phraseRepository.GetByIdAsync(wordInPhrase.InPhraseId).Result;
		    if (phrase == null)
		    {
			    _wordInPhraseRepository.Remove(wordInPhrase);
			    recordsAffected++;
		    }
		    
		    // if word is no longer within phrase, remove WordInPhrase
		    if (phrase != null && word != null)
		    {
			    // make sure the word is a discrete word and not part of another word
			    if (IsWordADiscreteWordWithinPhrase(word.TheWord, phrase.ThePhrase))
			    {
				    _wordInPhraseRepository.Remove(wordInPhrase);
				    recordsAffected++;
			    }
		    }
			
		    if (recordsAffected > 0)
		    {
			    _wordInPhraseRepository.SaveAllAsync().Wait();
		    }
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