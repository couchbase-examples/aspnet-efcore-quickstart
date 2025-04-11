using Couchbase.EFCore.Quickstart.Data;
using Couchbase.EFCore.Quickstart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Couchbase.EFCore.Quickstart.Controllers;

[ApiController]
[Route("/api/v1/route")]
public class RouteController: Controller
{
    private readonly TravelDbContext _context;
    private readonly ILogger<RouteController> _logger;

    public RouteController(TravelDbContext context, ILogger<RouteController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Description = "Get Route with specified ID.")]
    [SwaggerResponse(200, "Found Route")]
    [SwaggerResponse(404, "Route ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving route with ID {RouteId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    
    [HttpPost]
    [SwaggerOperation(Description = "Create Route with specified ID.")]
    [SwaggerResponse(201, "Created")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Post([FromBody] RouteCreateRequestCommand request)
    {
        try
        {
            var route = request.GetRoute();
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = route.Id }, route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating route");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Description = "Update Route with specified ID.")]
    [SwaggerResponse(200, "Route Updated")]
    [SwaggerResponse(404, "Route ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RouteUpdateRequestCommand request)
    {
        try
        {
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            request.GetUpdatedRoute(route);
            await _context.SaveChangesAsync();

            return Ok(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating route with ID {RouteId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Description = "Delete Route with specified ID.")]
    [SwaggerResponse(204, "Route Deleted")]
    [SwaggerResponse(404, "Route ID not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting route with ID {RouteId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}