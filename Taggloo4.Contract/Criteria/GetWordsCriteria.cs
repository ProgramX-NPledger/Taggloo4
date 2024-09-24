namespace Taggloo4.Contract.Criteria;

public class GetWordsCriteria : PaginationCriteria
{
    public int? DictionaryId { get; set; }
    public string? Query { get; set; }
    public WordsSortColumn SortBy { get; set; } = WordsSortColumn.WordId;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}