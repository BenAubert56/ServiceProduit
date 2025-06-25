using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProduit.Models;

namespace ServiceProduit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Comments)
                .ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Notable,
                AverageRating = p.Comments.Any() ? p.Comments.Average(c => c.Rating) : 0
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var result = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Notable,
                AverageRating = product.Comments.Any() ? product.Comments.Average(c => c.Rating) : 0,
                Comments = product.Comments.Select(c => new { c.Id, c.Text, c.Rating })
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updated)
        {
            if (id != updated.Id)
                return BadRequest();

            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            if (product.Comments.Any())
                return BadRequest("Cannot delete a product with comments.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
