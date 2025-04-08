using Couchbase.EFCore.Quickstart.Data;
using Couchbase.EFCore.Quickstart.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
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
                query = query.Where(a => a.Country.ToLower() == country.ToLower()).OrderBy(a=>a.Name).Skip(offset ?? 0).Take(limit ?? 10);
            }

            else
            {
                query = query.OrderBy(a => a.Name).Skip(offset ?? 0).Take(limit ?? 10);
            }
            
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
    
    [HttpGet("{id}")]
    [SwaggerOperation(Description = "Get Airline with specified ID.")]
    [SwaggerResponse(200, "Found Airline")]
    [SwaggerResponse(404, "Airline ID not found")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var airline =  await _context.Airlines.FirstOrDefaultAsync(r => r.Id == id);
        if (airline == null)
        {
          return NotFound();
        }

        return Ok(airline);
    }
    
    [HttpPost]
    [SwaggerOperation(Description = "Create Airline with specified ID.")]
    [SwaggerResponse(201, "Created")]
    public async Task<IActionResult> Post([FromBody] AirlineCreateRequestCommand request)
    {
        var airline = request.GetAirline();
        _context.Airlines.Add(airline);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = airline.Id }, airline);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Description = "Update Airline with specified ID.")]
    [SwaggerResponse(200, "Airline Updated")]
    [SwaggerResponse(404, "Airline ID not found")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AirlineUpdateRequestCommand request)
    {
        var airline = await _context.Airlines.FirstOrDefaultAsync(r=> r.Id == id);
        if (airline == null)
        {
            return NotFound();
        }

        request.GetUpdatedAirline(airline);

        await _context.SaveChangesAsync();
        
        return Ok(airline);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Description = "Delete Airline with specified ID.")]
    [SwaggerResponse(204, "Airline Deleted")]
    [SwaggerResponse(404, "Airline ID not found")]
    public async Task<IActionResult> Delete([FromRoute] int id)
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
}