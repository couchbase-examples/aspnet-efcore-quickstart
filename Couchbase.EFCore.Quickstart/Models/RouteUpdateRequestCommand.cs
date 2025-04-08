using System.Text.Json.Serialization;

namespace Couchbase.EFCore.Quickstart.Models;

public class RouteUpdateRequestCommand
{
    [JsonPropertyName("airline")]
    public string Airline { get; set; } = string.Empty;

    [JsonPropertyName("airlineid")]
    public string AirlineId { get; set; } = string.Empty;

    [JsonPropertyName("destinationairport")]
    public string DestinationAirport { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("equipment")]
    public string Equipment { get; set; } = string.Empty;

    [JsonPropertyName("schedule")]
    public List<Schedule> Schedule { get; set; }

    [JsonPropertyName("sourceairport")]
    public string SourceAirport { get; set; } = string.Empty;
    
    [JsonPropertyName("stops")]
    public int Stops { get; set; }

    public Route GetUpdatedRoute(Route existingRoute)
    {
        existingRoute.Airline = this.Airline;
        existingRoute.AirlineId = this.AirlineId;
        existingRoute.DestinationAirport = this.DestinationAirport;
        existingRoute.Distance = this.Distance;
        existingRoute.Equipment = this.Equipment;
        existingRoute.Schedule = this.Schedule;
        existingRoute.SourceAirport = this.SourceAirport;
        existingRoute.Stops = this.Stops;
        return existingRoute;
    }
}