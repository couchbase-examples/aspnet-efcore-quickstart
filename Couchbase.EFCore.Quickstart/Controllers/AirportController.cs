using Couchbase.EFCore.Quickstart.Data;
using Couchbase.EFCore.Quickstart.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Couchbase.EFCore.Quickstart.Controllers;

[ApiController]
[Route("/api/v1/airport")]
public class AirportController: Controller
{
    private readonly TravelDbContext _context;
    private readonly ILogger<AirportController> _logger;

    public AirportController(TravelDbContext context, ILogger<AirportController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("list")]
    [SwaggerOperation(Description = "Get list of Airports. Optionally, you can filter the list by Country.\n\nThis uses Couchbase EF Core to fetch a list of documents matching the specified criteria.\n\n Code: [`Controllers/AirportController`](https://github.com/couchbase-examples/aspnet-quickstart/blob/main/src/Org.Quickstart.API/Controllers/AirportController.cs) \n Class: `AirportController` \n Method: `List`")]
    [SwaggerResponse(200, "List of airports")]
    [SwaggerResponse(500, "Unexpected Error")]
    public async Task<IActionResult> List(
        [FromQuery(Name = "country"), SwaggerParameter("Country (Example: France, United Kingdom, United States)")] string? country,
        [FromQuery(Name = "limit"), SwaggerParameter("Number of airports to return (page size). Default value: 10.")] int? limit,
        [FromQuery(Name = "offset"), SwaggerParameter("Number of airports to skip (for pagination). Default value: 0.")] int? offset)
    {
        try
        {
            var query = _context.Airports.AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(a => a.Country.ToLower() == country.ToLower());
            }

            query = query.OrderBy(a => a.Airportname);

            query = query
                .Skip(offset ?? 0)
                .Take(limit ?? 10);

            var items = await query.ToListAsync();

            return items.Count == 0 ? NotFound() : Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
        }
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Description = "Get Airport with specified ID.")]
    [SwaggerResponse(200, "Found Airport")]
    [SwaggerResponse(404, "Airport ID not found")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var airport = await _context.Airports.FirstOrDefaultAsync(a => a.Id == id);
            if (airport == null)
            {
                return NotFound();
            }
            return Ok(airport);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
   
    [HttpPost]
    [SwaggerOperation(Description = "Create Airline with specified ID.")]
    [SwaggerResponse(201, "Created")]
    public async Task<IActionResult> Post([FromBody] AirportCreateRequestCommand request)
    {
        try
        {
            var airport = request.GetAirport();
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = airport.Id }, airport);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Description = "Update Airport with specified ID.")]
    [SwaggerResponse(200, "Airport Updated")]
    [SwaggerResponse(404, "Airport ID not found")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AirportUpdateRequestCommand request)
    {
        try
        {
            var airport = await _context.Airports.FirstOrDefaultAsync(a => a.Id == id);
            if (airport == null)
            {
                return NotFound();
            }
            request.GetUpdatedAirport(airport);
            await _context.SaveChangesAsync();
            return Ok(airport);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Description = "Delete Airport with specified ID.")]
    [SwaggerResponse(204, "Airport Deleted")]
    [SwaggerResponse(404, "Airport ID not found")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var airport = await _context.Airports.FirstOrDefaultAsync(a => a.Id == id);
            if (airport == null)
            {
                return NotFound();
            }
            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {Message}", ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}