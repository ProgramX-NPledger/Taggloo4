using Taggloo4.Contract.Criteria;
using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words;

public class IndexViewModel
{
    public IEnumerable<WordInDictionary> Results { get; set; }
    public int CurrentPageNumber { get; set; }
    public int NumberOfPages { get; set; }

    public int TotalUnpagedItems { get; set; }

    public int ItemsPerPage { get; set; }

    public WordsSortColumn SortBy { get; set; }

    public SortDirection SortDirection { get; set; }
    
    
    
}