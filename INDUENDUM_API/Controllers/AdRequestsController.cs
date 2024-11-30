using INDUENDUM_API.Data;
using INDUENDUM_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdRequestsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetAllAdRequests()
    {
        var adRequests = await _context.AdRequests.Include(ar => ar.Company).ToListAsync();
        return Ok(adRequests);
    }

    
    [HttpPost]
    [Authorize(Roles = "Company")] 
    public async Task<IActionResult> CreateAdRequest([FromBody] AdRequest adRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            
            var companyExists = await _context.Users.OfType<ApplicationUser>().AnyAsync(u => u.Id == adRequest.CompanyId);
            if (!companyExists)
            {
                return NotFound("Kompania nuk ekziston.");
            }

            _context.AdRequests.Add(adRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllAdRequests), new { id = adRequest.Id }, adRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gabim i brendshëm: {ex.Message}");
        }
    }

    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> UpdateAdRequest(int id, [FromBody] AdRequest adRequest)
    {
        if (id != adRequest.Id)
        {
            return BadRequest("Id e dhënë nuk përputhet me objektin.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _context.Entry(adRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdRequestExists(id))
            {
                return NotFound("Kërkesa nuk ekziston.");
            }

            throw;
        }
    }

    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteAdRequest(int id)
    {
        var adRequest = await _context.AdRequests.FindAsync(id);
        if (adRequest == null)
        {
            return NotFound("Kërkesa nuk u gjet.");
        }

        _context.AdRequests.Remove(adRequest);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool AdRequestExists(int id)
    {
        return _context.AdRequests.Any(e => e.Id == id);
    }
}
