using INDUENDUM_API.Models;
using INDUENDUM_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Kërkon autentifikim për të gjitha metodat
public class CollectionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CollectionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/collections/{userId}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCollections(string userId) // Ndrysho në string për të përputhur ApplicationUser.Id
    {
        var collections = await _context.Collections
            .Where(c => c.UserId == userId)
            .Include(c => c.Products) // Përfshi detajet e produkteve
            .ToListAsync();

        if (collections == null || collections.Count == 0)
            return NotFound("Nuk u gjetën koleksione për këtë përdorues.");

        return Ok(collections);
    }

    // POST: /api/collections
    [HttpPost]
    [Authorize(Roles = "User")] // Vetëm përdoruesit e thjeshtë mund të krijojnë koleksione
    public async Task<IActionResult> CreateCollection([FromBody] Collection collection)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Kontrollo nëse përdoruesi ekziston
        var userExists = await _context.Users.AnyAsync(u => u.Id == collection.UserId);
        if (!userExists)
            return NotFound("Përdoruesi nuk ekziston.");

        _context.Collections.Add(collection);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCollections), new { userId = collection.UserId }, collection);
    }

    // DELETE: /api/collections/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "User")] // Vetëm përdoruesit e thjeshtë mund të fshijnë koleksionet e tyre
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var collection = await _context.Collections
            .Include(c => c.Products) // Përfshi produktet për fshirje kaskadë
            .FirstOrDefaultAsync(c => c.Id == id);

        if (collection == null)
            return NotFound("Koleksioni nuk u gjet.");

        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

