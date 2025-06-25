using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceCommentaire.Models;

namespace ServiceCommentaire.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetComment(int id)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                return NotFound();

            var result = new
            {
                comment.Id,
                comment.Text,
                comment.Rating
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpGet("product/{productId}/average")]
        public async Task<ActionResult<double>> GetAverageRating(int productId)
        {
            var comments = await _context.Comments
                .Where(c => c.ProductId == productId)
                .ToListAsync();

            if (!comments.Any())
                return Ok(0);

            return Ok(comments.Average(c => c.Rating));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, Comment updated)
        {
            if (id != updated.Id)
                return BadRequest();

            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
