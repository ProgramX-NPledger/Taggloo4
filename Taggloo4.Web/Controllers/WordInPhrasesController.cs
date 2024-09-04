using System.Collections.ObjectModel;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taggloo4.Dto;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Model;

namespace Taggloo4.Web.Controllers;

/// <summary>
/// User operations. All methods require authorisation.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WordInPhrasesController : BaseApiController
{
	private readonly IWordInPhraseRepository _wordInPhraseRepository;

	/// <summary>
	/// Constructor with injected parameters.
	/// </summary>
	/// <param name="wordInPhraseRepository">Implementationo of <see cref="IWordInPhraseRepository"/>.</param>
	public WordInPhrasesController(IWordInPhraseRepository wordInPhraseRepository)
	{
		_wordInPhraseRepository = wordInPhraseRepository;
	}
	
	
	/// <summary>
	/// Return a Word in Phrase by ID.
	/// </summary>
	/// <param name="id">ID of the Word in Phrase to return.</param>
	/// <response code="200">Phrase returned.</response>
	/// <response code="404">Word in Phrase not found.</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "administrator,dataExporter")]
	public async Task<ActionResult<GetWordInPhraseResultItem>> GetWordInPhraseById(int id)
	{
		WordInPhrase? wordInPhrase = await _wordInPhraseRepository.GetByIdAsync(id);
		if (wordInPhrase == null) return NotFound();

		return Ok(new GetWordInPhraseResultItem()
		{
			CreatedOn = wordInPhrase.CreatedOn,
			ThePhrase = wordInPhrase.InPhrase.ThePhrase,
			TheWord = wordInPhrase.Word.TheWord,
			CreatedByUserName = wordInPhrase.CreatedByUserName,
			CreatedAt = wordInPhrase.CreatedAt,
			Id = wordInPhrase.Id,
			Ordinal = wordInPhrase.Ordinal,
			InPhraseId = wordInPhrase.InPhraseId,
			WordId = wordInPhrase.WordId,
			Links = new[]
			{
				new Link()
				{
					Action = "get",
					Rel = "self",
					HRef = $"{GetBaseApiPath()}/wordinphrases/{wordInPhrase.Id}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "word",
					HRef = $"{GetBaseApiPath()}/words/{wordInPhrase.WordId}",
					Types = new[] { JSON_MIME_TYPE }
				},
				new Link()
				{
					Action = "get",
					Rel = "phrase",
					HRef = $"{GetBaseApiPath()}/phrases/{wordInPhrase.InPhraseId}",
					Types = new[] { JSON_MIME_TYPE }
				}
			}
		});
	}


	/// <summary>
	/// Retrieve matching Word in Phrases.
	/// </summary>
	/// <param name="phraseId">If specified, returns Words in Phrases for the specified Phrase.</param>
	/// <param name="offsetIndex">If specified, returns results starting at the specified offset position (starting index 0) Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="pageSize">If specified, limits the number of results to the specified limit. Default is defined by <seealso cref="Defaults.OffsetIndex"/>.</param>
	/// <param name="wordId">If specified, returns Words in Phrases for the specified Word.</param>
	/// <response code="200">Results prepared.</response>
	/// <response code="403">Not permitted.</response>
	[HttpGet()]
	[Authorize(Roles="administrator, dataExporter")]
	public async Task<ActionResult<GetWordInPhrasesResult>> GetPhrases(
		int? wordId,
		int? phraseId,
		int offsetIndex=Defaults.OffsetIndex, 
		int pageSize = Defaults.MaxItems)
	{
		AssertApiConstraints(pageSize);
		
		DateTime start = DateTime.Now;
		
		IEnumerable<WordInPhrase> phrases = (await _wordInPhraseRepository.GetWordsInPhrasesAsync(wordId, phraseId)).ToArray();

		List<Link> links = new List<Link>();
		links.Add(new Link()
		{
			Action = "get",
			Rel = "self",
			Types = new[] { JSON_MIME_TYPE },
			HRef = BuildPageNavigationUrl(wordId,phraseId,offsetIndex, pageSize)
		});
		if (offsetIndex > 0)
		{
			if (offsetIndex - pageSize < 0)
			{
				// previous page, but offset was incorrect
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(wordId,phraseId,0, pageSize),
					Types = new[] { JSON_MIME_TYPE }
				});
			}
			else
			{
				links.Add(new Link()
				{
					Action = "get",
					Rel = "previouspage",
					HRef = BuildPageNavigationUrl(wordId,phraseId,offsetIndex - pageSize, pageSize),
					Types = new [] { JSON_MIME_TYPE }
				});
			}

		}
		
		decimal numberOfPages = Math.Ceiling(phrases.Count()/(decimal)pageSize);
		decimal offsetIndexOfLastPage = numberOfPages * pageSize;
		decimal remainder = numberOfPages % pageSize;
		offsetIndexOfLastPage -= remainder;

		if (offsetIndexOfLastPage <= phrases.Count())
		{
			links.Add(new Link()
			{
				Action = "get",
				Rel = "nextpage",
				HRef = BuildPageNavigationUrl(wordId,phraseId, offsetIndex + pageSize, pageSize),
				Types = new [] { JSON_MIME_TYPE }
			});
		}
		
		GetWordInPhrasesResult getWordInPhrasesResult = new GetWordInPhrasesResult()
		{
			Results = phrases.Skip(offsetIndex).Take(pageSize).Select(p => new GetWordInPhraseResultItem()
			{
				Id = p.Id,
				CreatedAt = p.CreatedAt,
				CreatedOn = p.CreatedOn,
				CreatedByUserName = p.CreatedByUserName,
				ThePhrase = p.InPhrase.ThePhrase,
				TheWord = p.Word.TheWord,
				Ordinal = p.Ordinal,
				InPhraseId = p.InPhraseId,
				WordId = p.WordId,
				Links = new[]
				{
					new Link()
					{
						Action = "get",
						Rel = "self",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/phrases/{p.Id}"
					},
					new Link()
					{
						Action = "get",
						Rel = "word",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/words/{p.WordId}"
					},
					new Link()
					{
						Action = "get",
						Rel = "phrase",
						Types = new[] { JSON_MIME_TYPE },
						HRef = $"{GetBaseApiPath()}/phrases/{p.InPhraseId}"
					}
				}
			}),
			Links = links.ToArray(),
			FromIndex = offsetIndex,
			PageSize = pageSize,
			TotalItemsCount = phrases.Count(),
			DeltaMs = (DateTime.Now-start).TotalMilliseconds
		};
		return getWordInPhrasesResult;
	}
	
	private string BuildPageNavigationUrl(int? wordId, int? phraseId,  int offsetIndex, int pageSize)
	{
		StringBuilder sb = new StringBuilder(GetBaseApiPath() + "/wordInPhrases?");
		if (wordId.HasValue) sb.Append($"wordId={wordId}&");
		if (phraseId.HasValue) sb.Append($"phraseId={phraseId}&");
		sb.Append($"offsetIndex={offsetIndex}&");
		if (pageSize != Defaults.MaxItems) sb.Append($"pageSize={pageSize}");
		return sb.ToString();
	}
	
	
}