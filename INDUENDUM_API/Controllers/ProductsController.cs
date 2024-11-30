using INDUENDUM_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Siguron që endpointet kërkojnë autentifikim
public class ProductsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ProductsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // GET: /api/products
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = new List<Product>();

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Products", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                CompanyId = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gabim i brendshëm gjatë marrjes së produkteve: {ex.Message}");
        }
    }

    // POST: /api/products
    [HttpPost]
    [Authorize(Roles = "Admin,Company")] // Admin dhe Kompanitë mund të shtojnë produkte
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand("AddProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                    cmd.Parameters.AddWithValue("@CompanyId", product.CompanyId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok("Produkti u shtua me sukses.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gabim i brendshëm gjatë krijimit të produktit: {ex.Message}");
        }
    }

    // GET: /api/products/by-company/{companyId}
    [HttpGet("by-company/{companyId}")]
    public async Task<IActionResult> GetProductsByCompany(string companyId)
    {
        try
        {
            var products = new List<Product>();

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand("GetProductsByCompany", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                ImageUrl = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                CompanyId = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gabim i brendshëm gjatë marrjes së produkteve: {ex.Message}");
        }
    }

    // DELETE: /api/products/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Company")] // Admin dhe Kompanitë mund të fshijnë produkte
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand("DeleteProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductId", id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok("Produkti u fshi me sukses.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Gabim i brendshëm gjatë fshirjes së produktit: {ex.Message}");
        }
    }
}
