namespace Taggloo4.Web.Model;

/// <summary>
/// Represents an instance of a re-indexing job.
/// </summary>
public class ReindexingJob
{
    /// <summary>
    /// Identifier of Job.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Timestamp Job was started.
    /// </summary>
    public DateTime StartedAt { get; set; }
    
    /// <summary>
    /// Timestamp Job was completed.
    /// </summary>
    public DateTime? FinishedAt { get; set; }
    
    /// <summary>
    /// Username of starting user.
    /// </summary>
    public required string StartedByUserName { get; set; }
    
    /// <summary>
    /// Machine name on which job was started.
    /// </summary>
    public required string StartedOn { get; set; }
    
    /// <summary>
    /// Number of <see cref="Language"/>s processed.
    /// </summary>
    public int? NumberOfLanguagesProcessed { get; set; }
    
    /// <summary>
    /// Number of <see cref="Dictionary"/> items processed.
    /// </summary>
    public int? NumberOfDictionariesProcessed { get; set; }
    
    /// <summary>
    /// Number of <see cref="Phrase"/>s processed.
    /// </summary>
    public int? NumberOfPhrasesProcessed { get; set; }
    
    /// <summary>
    /// Number of <see cref="Word"/>s processed.
    /// </summary>
    public int? NumberOfWordProcessed { get; set; }
    
    /// <summary>
    /// Number of <see cref="WordInPhrase"/>s created.
    /// </summary>
    public int? NumberOfWordsInPhrasesCreated { get; set; }

    /// <summary>
    /// Number of <see cref="Word"/>s created.
    /// </summary>
    public int? NumberOfWordsCreated { get; set; }

    /// <summary>
    /// Number of <see cref="WordInPhrase"/>s removed due to redundant links.
    /// </summary>
    public int? NumberOfWordsInPhrasesRemoved { get; set; }
    
    
}