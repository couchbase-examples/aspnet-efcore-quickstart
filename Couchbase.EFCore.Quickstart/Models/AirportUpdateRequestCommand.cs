using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class AirportUpdateRequestCommand
{
    [JsonPropertyName("airportname")] 
    public string Airportname { get; set; } = string.Empty;
    
    [JsonPropertyName("city")] 
    public string City { get; set; } = string.Empty;
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("faa")] 
    public string Faa { get; set; } = string.Empty;

    [JsonPropertyName("geo")] 
    public Geo Geo { get; set; }
    
    [JsonPropertyName("icao")]
    public string Icao { get; set; } = string.Empty;

    [JsonPropertyName("tz")] 
    public string Tz { get; set; } = string.Empty;

    public Airport GetUpdatedAirport (Airport existingAirport)
    {
        existingAirport.Airportname = this.Airportname;
        existingAirport.City = this.City;
        existingAirport.Country = this.Country;
        existingAirport.Faa = this.Faa;
        existingAirport.Geo = this.Geo;
        existingAirport.Icao = this.Icao;
        existingAirport.Tz = this.Tz;
        return existingAirport;
    }
}