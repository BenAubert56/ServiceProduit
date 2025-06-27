using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLecture.Models;

namespace ServiceLecture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("product/{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            var comments = await _context.Comments.Where(c => c.ProductId == id).ToListAsync();
            var rating = comments.Any() ? comments.Average(c => c.Rating) : 0.0;
            return Ok(new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Notable,
                Rating = rating,
                Comments = comments.Select(c => new { c.Id, c.Text, c.Rating })
            });
        }
    }
}
