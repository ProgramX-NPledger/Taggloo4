using System.Collections.ObjectModel;
using API.Contract;
using API.Model;
using Hangfire;
using Taggloo4.Dto;
using Taggloo4.Model.Exceptions;

namespace API.Jobs;

public class ReindexJob
{
	private readonly IWordRepository _wordRepository;
	private readonly IPhraseRepository _phraseRepository;
	private readonly IDictionaryRepository _dictionaryRepository;

	public ReindexJob()
	{
		
	}
	
	public ReindexJob(IWordRepository wordRepository,
	    IPhraseRepository phraseRepository,
	    IDictionaryRepository dictionaryRepository)
	{
		_wordRepository = wordRepository;
		_phraseRepository = phraseRepository;
		_dictionaryRepository = dictionaryRepository;
	}

	
    public void Reindex(string remoteHostAddress,
	    string? currentUserName,
	    string theWord,
	    int dictionaryId, 
	    Guid importGuid,
	    DateTime? timeStamp=null)
    {
	   
		
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