namespace API.Contract;

public interface IDatabaseManagement
{
    decimal GetDatabaseSize();
    void ShrinkDatabase();

    int DeleteOldestLogsByDay();

    int DeleteLogsByPropertiesText(IEnumerable<string> textStrings);
    

}