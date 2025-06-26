using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProduit.Models;
using System.Net.Http.Json;
using System.Net.Http;
using Polly;

namespace ServiceProduit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public ProductController(AppDbContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            double rating = 0;
            var client = _clientFactory.CreateClient("service-commentaire");
            var fallback = Policy<double>
                .Handle<Exception>()
                .FallbackAsync(0d);

            rating = await fallback.ExecuteAsync(async () =>
            {
                return await client.GetFromJsonAsync<double>($"api/comment/product/{id}/average");
            });

            var result = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Notable,
                Rating = rating
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
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
