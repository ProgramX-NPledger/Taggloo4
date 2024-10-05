namespace Taggloo4.Contract.Criteria;

public class GetDictionariesCriteria : PaginationCriteria
{
    public string? Query { get; set; }

    public string? ContentTypeKey { get; set; }
    public string? IetfLanguageTag { get; set; }
    public DictionariesSortColumn SortBy { get; set; } = DictionariesSortColumn.DictionaryId;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}