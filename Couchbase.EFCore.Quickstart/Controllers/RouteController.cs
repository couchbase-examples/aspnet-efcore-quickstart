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
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var route =  await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);
        if (route == null)
        {
            return NotFound();
        }
        
        return Ok(route);
    }
    
    [HttpPost]
    [SwaggerOperation(Description = "Create Route with specified ID.")]
    [SwaggerResponse(201, "Created")]
    public async Task<IActionResult> Post([FromBody] RouteCreateRequestCommand request)
    {
        var route = request.GetRoute();
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = route.Id }, route);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Description = "Update Route with specified ID.")]
    [SwaggerResponse(200, "Route Updated")]
    [SwaggerResponse(404, "Route ID not found")]    
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RouteUpdateRequestCommand request)
    {
        var route = await _context.Routes.FirstOrDefaultAsync(r=> r.Id == id);
        if (route == null)
        {
            return NotFound();
        }

        request.GetUpdatedRoute(route);

        await _context.SaveChangesAsync();
    
        return Ok(route);
    }
    
    [HttpDelete("{id}")]
    [SwaggerOperation(Description = "Delete Route with specified ID.")]
    [SwaggerResponse(204, "Route Deleted")]
    [SwaggerResponse(404, "Route ID not found")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var route = await _context.Routes.FirstOrDefaultAsync(r=>r.Id == id);
        if (route == null)
        {
            return NotFound();
        }
        
        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}