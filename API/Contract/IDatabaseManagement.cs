namespace API.Contract;

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
    

}