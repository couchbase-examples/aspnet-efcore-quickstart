using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class Schedule
{
    [JsonPropertyName("day")]
    public int Day { get; set; }
    
    [JsonPropertyName("utc")]
    public string Utc { get; set; } = string.Empty;
    
    [JsonPropertyName("flight")]
    public string Flight { get; set; } = string.Empty;
}