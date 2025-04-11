using Couchbase.EFCore.Quickstart.Data;
using Couchbase.EFCore.Quickstart.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Couchbase.EFCore.Quickstart.Controllers;

[ApiController]
[Route("/api/v1/airline")]
public class AirlineController: Controller
{
    private readonly TravelDbContext _context;
    private readonly ILogger<AirlineController> _logger;

    public AirlineController(TravelDbContext context, ILogger<AirlineController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet("list")]
    [SwaggerOperation(Description =
        "Get list of Airlines. Optionally, you can filter the list by Country.\n\nThis provides an example of using EFCore with Couchbase to fetch a list of documents matching the specified criteria.\n\nClass: `AirlineController` \nMethod: `List`")]
    [SwaggerResponse(200, "List of airlines")]
    [SwaggerResponse(500, "Unexpected Error")]
    public async Task<IActionResult> List(
        [FromQuery(Name = "country"),
         SwaggerParameter("Country (Example: France, United Kingdom, United States)")]
        string? country,
        [FromQuery(Name = "limit"),
         SwaggerParameter("Number of airlines to return (page size). Default value: 10.")]
        int? limit,
        [FromQuery(Name = "offset"),
         SwaggerParameter("Number of airlines to skip (for pagination). Default value: 0.")]
        int? offset)
    {
        try
        {
            var query = _context.Airlines.AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(a => a.Country.ToLower() == country.ToLower());
            }
        
            query = query.OrderBy(a => a.Name).Skip(offset ?? 0).Take(limit ?? 10);
            
            var items = await query.ToListAsync();
            
            return items.Count == 0 ? NotFound() : Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
        }
    }
    
    [HttpGet]
    [Route("to-airport")]
    [SwaggerOperation(Description =
        "Get Airlines flying to the specified destination Airport.\n\n" +
        "This demonstrates using EF Core to fetch documents matching specified criteria.\n\n" +
        "Code: [`Controllers/AirlineController`](https://github.com/couchbase-examples/aspnet-quickstart/blob/main/src/Org.Quickstart.API/Controllers/AirlineController.cs)\n" +
        "Class: `AirlineController`\n" +
        "Method: `ToAirport`")]
    [SwaggerResponse(200, "List of airlines")]
    [SwaggerResponse(500, "Unexpected Error")]
    public async Task<IActionResult> ToAirport(
        [FromQuery(Name = "airport"), SwaggerParameter("Destination airport (Example: SFO, JFK, LAX)", Required = true)]
        string airport,
        [FromQuery(Name = "limit"), SwaggerParameter("Number of airlines to return (page size). Default: 10.")]
        int? limit,
        [FromQuery(Name = "offset"), SwaggerParameter("Number of airlines to skip (for pagination). Default: 0.")]
        int? offset)
    {
        try
        {
            var pageSize = limit ?? 10;
            var skip = offset ?? 0;
            var airportCode = airport.ToLowerInvariant();

            var sql = $@"
                SELECT air.callsign,
                       air.country,
                       air.iata,
                       air.icao,
                       air.name
                FROM (
                    SELECT DISTINCT META(airline).id AS airlineId
                    FROM `travel-sample`.`inventory`.`route` AS route
                    JOIN `travel-sample`.`inventory`.`airline` AS airline ON route.airlineid = META(airline).id
                    WHERE LOWER(route.destinationairport) = {{0}}
                ) AS SUBQUERY
                JOIN `travel-sample`.`inventory`.`airline` AS air ON META(air).id = SUBQUERY.airlineId
                LIMIT {{1}}
                OFFSET {{2}}";

            var airlines = await _context.Airlines
                .FromSqlRaw(sql, airportCode, pageSize, skip)
                .ToListAsync();

            if (airlines.Count == 0)
            {
                return NotFound("No airlines found for the specified airport.");
            }

            return Ok(airlines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching airlines for airport: {Airport}", airport);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace,
                request = Request.GetDisplayUrl()
            });
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Description = "Get Airline with specified ID.")]
    [SwaggerResponse(200, "Found Airline")]
    [SwaggerResponse(404, "Airline ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            var airline = await _context.Airlines.FirstOrDefaultAsync(r => r.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving airline with ID {Id}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    
    [HttpPost]
    [SwaggerOperation(Description = "Create Airline with specified ID.")]
    [SwaggerResponse(201, "Created")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Post([FromBody] AirlineCreateRequestCommand request)
    {
        try
        {
            var airline = request.GetAirline();
            _context.Airlines.Add(airline);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = airline.Id }, airline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating airline with request {@Request}", request);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Description = "Update Airline with specified ID.")]
    [SwaggerResponse(200, "Airline Updated")]
    [SwaggerResponse(404, "Airline ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AirlineUpdateRequestCommand request)
    {
        try
        {
            var airline = await _context.Airlines.FirstOrDefaultAsync(r => r.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            request.GetUpdatedAirline(airline);

            await _context.SaveChangesAsync();

            return Ok(airline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating airline with ID {AirlineId} and request {@Request}", id, request);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Description = "Delete Airline with specified ID.")]
    [SwaggerResponse(204, "Airline Deleted")]
    [SwaggerResponse(404, "Airline ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var airline = await _context.Airlines.FirstOrDefaultAsync(r => r.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            _context.Airlines.Remove(airline);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting airline with ID {AirlineId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}