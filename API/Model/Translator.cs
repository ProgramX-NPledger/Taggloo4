using System.ComponentModel.DataAnnotations;

namespace API.Model;

public class Translator
{
    [Key]
    public required string Key { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string DotNetFactoryType { get; set; }
    public required string DotNetFactoryAssembly { get; set; }
    
}