using INDUENDUM_API.Models;
using INDUENDUM_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Çdo endpoint kërkon autentifikim
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /api/users
    [HttpGet]
    [Authorize(Roles = "Admin")] // Vetëm Admin-i mund të marrë listën e përdoruesve
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync(); // Merr listën e përdoruesve nga UserManager
        return Ok(users);
    }

    // POST: /api/users
    [HttpPost]
    [AllowAnonymous] // Lejon krijimin e përdoruesve pa autentifikim
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Kontrollo nëse email-i ekziston
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
            return Conflict("Ky email është tashmë i regjistruar.");

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName
        };

        // Krijo përdoruesin me fjalëkalimin e tij
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest($"Gabime gjatë krijimit të përdoruesit: {errors}");
        }

        // Shto rolin default 'Consumer' nëse nuk specifikohet ndonjë tjetër
        if (string.IsNullOrEmpty(model.Role))
        {
            model.Role = "Consumer";
        }

        await _userManager.AddToRoleAsync(user, model.Role);

        return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user);
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Vetëm admini mund të fshijë përdorues
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("Përdoruesi nuk u gjet.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest($"Gabime gjatë fshirjes së përdoruesit: {errors}");
        }

        return NoContent();
    }
}

// Model për regjistrimin e përdoruesve
public class RegisterUserModel
{
    [Required(ErrorMessage = "Email-i është i detyrueshëm.")]
    [EmailAddress(ErrorMessage = "Ju lutemi vendosni një email të vlefshëm.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Fjalëkalimi është i detyrueshëm.")]
    [MinLength(6, ErrorMessage = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emri i plotë është i detyrueshëm.")]
    public string FullName { get; set; } = string.Empty;

    public string? Role { get; set; } // Roli opsional (default = Consumer)
}
