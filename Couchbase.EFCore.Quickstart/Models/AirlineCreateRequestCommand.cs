using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class AirlineCreateRequestCommand
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("callsign")]
    public string Callsign { get; set; } = string.Empty;
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("iata")]
    public string Iata { get; set; } = string.Empty;
    
    [JsonPropertyName("icao")]
    public string Icao { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    public Airline GetAirline()
    {
        return new Airline()
        {
            Id = this.Id,
            Callsign = this.Callsign,
            Country = this.Country,
            Iata = this.Iata,
            Icao = this.Icao,
            Name = this.Name
        };
    }
}