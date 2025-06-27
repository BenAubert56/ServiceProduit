using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProduit.Models;
using Steeltoe.Messaging.RabbitMQ.Core;
using ServiceProduit.Events;

namespace ServiceProduit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitTemplate _rabbitTemplate;

        public ProductController(AppDbContext context, RabbitTemplate rabbitTemplate)
        {
            _context = context;
            _rabbitTemplate = rabbitTemplate;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var evt = new ProductCreatedEvent(product.Id, product.Name, product.Price);
            _rabbitTemplate.ConvertAndSend("ms.produit", "product.created", evt);
            return Created(string.Empty, product);
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
