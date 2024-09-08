using Taggloo4.Model;

namespace Taggloo4.Contract;

/// <summary>
/// Contract for database maintenance tasks.
/// </summary>
public interface IDatabaseManagement
{
    /// <summary>
    /// Returns the database size in Megabytes.
    /// </summary>
    /// <returns>The database size in Megabytes.</returns>
    decimal GetDatabaseSize();
    
    /// <summary>
    /// Shrinks the database using known tooling of the DBMS.
    /// </summary>
    void ShrinkDatabase();

    /// <summary>
    /// Deletes old logs.
    /// </summary>
    /// <returns>Number of records deleted.</returns>
    int DeleteOldestLogsByDay();

    /// <summary>
    /// Deletes logs by text strings.
    /// </summary>
    /// <param name="textStrings">A string-array containing text strings that may appear in log messages that should be deleted.</param>
    /// <returns>Number of records deleted.</returns>
    int DeleteLogsByPropertiesText(IEnumerable<string> textStrings);

    /// <summary>
    /// Retrieves Reindexing Jobs.
    /// </summary>
    /// <param name="isActive">When specified, filters by status of execution status.</param>
    /// <returns>A collection of <see cref="ReindexingJob"/> items.</returns>
    Task<IEnumerable<ReindexingJob>> GetReindexingJobsAsync(bool? isActive);

    /// <summary>
    /// Marks a re-indexing job as being started.
    /// </summary>
    /// <param name="userName">Username of starting user.</param>
    /// <param name="hostName">Name of machine on which job was started.</param>
    /// <returns>The identifier of the reindexing job.</returns>
    Task<int> StartReindexingJobAsync(string userName, string hostName);

    /// <summary>
    /// Marks a re-indexing job as being completed.
    /// </summary>
    /// <param name="reindexingJobId">The identifier of the job to complete. This is provided by the <see cref="StartReindexingJobAsync"/> function.</param>
    /// <param name="numberOfLanguagesProcessed">Number of <see cref="Language"/>s processed.</param>
    /// <param name="numberOfDictionariesProcessed">Number of <see cref="Dictionary"/> items processed.</param>
    /// <param name="numberOfPhrasesProcessed">Number of <see cref="Phrase"/>s processed.</param>
    /// <param name="numberOfWordsProcessed">Number of <see cref="Word"/>s processed.</param>
    /// <param name="numberOfWordsCreated">Number of <see cref="Word"/>s that were created.</param>
    /// <param name="numberOfWordsInPhrasesCreated">Number of <see cref="WordInPhrase"/> items created.</param>
    /// <param name="numberOfWordInPhrasesRemoved">Number of <see cref="WordInPhrase"/> items removed due to removal of links.</param>
    /// <returns><c>True</c> if successful.</returns>
    Task<bool> CompleteReindexingJobAsync(int reindexingJobId,
        int numberOfLanguagesProcessed,
        int numberOfDictionariesProcessed,
        int numberOfPhrasesProcessed,
        int numberOfWordsProcessed,
        int numberOfWordsCreated,
        int numberOfWordsInPhrasesCreated,
        int numberOfWordInPhrasesRemoved);
}