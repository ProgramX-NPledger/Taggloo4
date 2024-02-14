using System.Text.Json.Serialization;

namespace API.Data.Model;

public class SiteInitialisationConfiguration
{
    public User[] Users { get; set; }
    public Language[] Languages { get; set; }
    
}