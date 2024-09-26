using Taggloo4.Model;

namespace Taggloo4.Web.Areas.Admin.ViewModels.Words;

public class IndexViewModel
{
    public IEnumerable<WordInDictionary> Results { get; set; }
    public int CurrentPageNumber { get; set; }
    public int NumberOfPages { get; set; }

    public int TotalUnpagedItems { get; set; }

    public int ItemsPerPage { get; set; }
    

    
}