using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class Geo
{
    [JsonPropertyName("alt")]
    public double Alt { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}