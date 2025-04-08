using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class DestinationAirport
{
    [JsonPropertyName("destinationairport")]
    public string Destinationairport { get; set; } = string.Empty;
}