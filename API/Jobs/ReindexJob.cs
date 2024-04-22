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

	public ReindexJob()
	{
		
	}
	
	public ReindexJob(ILanguageRepository _languageRepository,
		IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository,
		IWordInPhraseRepository wordInPhraseRepository)
	{
		this._languageRepository = _languageRepository;
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
		_wordInPhraseRepository = wordInPhraseRepository;
	}

	
    public async void ReindexAsync(string remoteHostAddress,
	    string? currentUserName,
	    string theWord,
	    Guid importGuid,
	    DateTime? timeStamp=null)
    {
	    // for each language
	    IEnumerable<Language> allLanguages = await _languageRepository.GetAllLanguagesAsync();

	    foreach (Language language in allLanguages)
	    {
		    IEnumerable<Dictionary> allDictionariesInLanguage = await _dictionaryRepository.GetDictionariesAsync(null, language.IetfLanguageTag);

		    foreach (Dictionary dictionary in allDictionariesInLanguage)
		    {
			    IEnumerable<Phrase> allPhrasesInDictionary = await _phraseRepository.GetPhrasesAsync(null, dictionary.Id, null, null, null);

			    foreach (Phrase phrase in allPhrasesInDictionary)
			    {
				    // break apart phrase into words
				    string[] words = phrase.ThePhrase.Split(new char[] { ' ' }).Select(q => q.Trim()).ToArray();

				    foreach (string wordString in words)
				    {
					    IList<Word> matchingWords = (await _wordRepository.GetWordsAsync(wordString, dictionary.Id, null)).ToList();
					    if (!matchingWords.Any())
					    {
						    // no matching words, so create it and add it to the collection
						    Word newWord = new Word()
						    {
							    CreatedAt = DateTime.Now,
							    CreatedOn =Environment.MachineName,
							    TheWord = wordString,
							    CreatedByUserName = $"{Environment.UserDomainName}\\{Environment.UserName}",
							    Dictionary = dictionary
						    };
						    _wordRepository.Create(newWord);
						    await _wordRepository.SaveAllAsync();
						    matchingWords.Add(newWord);
					    }

					    int ordinal = 0;
					    foreach (Word word in matchingWords)
					    {
						    ordinal++;
						    
						    // there should only be one word in the collection, but if there are >1, they are all relevant
										
						    // is there already a WordInPhrase entry?
						    IEnumerable<WordInPhrase> wordInPhrases = await _wordRepository.GetPhrasesForWordAsync(word.Id);

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
							    await _wordRepository.SaveAllAsync();
						    }
					    } // foreach Word
				    } // foreach wordString
			    }// foreach Phrase
		    }// foreach Dictionary
	    }// foreach Language

	    IEnumerable<WordInPhrase> wordsInPhrases = await _wordInPhraseRepository.GetWordsInPhrasesAsync();
	    foreach (WordInPhrase wordInPhrase in wordsInPhrases)
	    {
		    int recordsAffected = 0;
		    
		    Word? word = await _wordRepository.GetByIdAsync(wordInPhrase.WordId);
		    if (word == null)
		    {
			    _wordInPhraseRepository.Remove(wordInPhrase);
			    recordsAffected++;
		    }

		    Phrase? phrase = await _phraseRepository.GetByIdAsync(wordInPhrase.InPhraseId);
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
			    await _wordInPhraseRepository.SaveChangesAsync();
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