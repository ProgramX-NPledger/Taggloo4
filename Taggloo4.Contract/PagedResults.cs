namespace Taggloo4.Contract;

public class PagedResults<T>
{
    public IEnumerable<T> Results { get; set; } = [];
    public int TotalUnpagedItems { get; set; }
}