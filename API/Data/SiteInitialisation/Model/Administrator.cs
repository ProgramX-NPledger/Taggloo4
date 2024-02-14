using System.Text.Json.Serialization;

namespace API.Data.Model;

public class Administrator
{
    [JsonPropertyName("Username")]
    public string UserName { get; set; }
    public string Password { get; set; }
}