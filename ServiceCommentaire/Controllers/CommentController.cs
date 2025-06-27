using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceCommentaire.Models;
using Steeltoe.Messaging.RabbitMQ.Core;
using ServiceCommentaire.Events;

namespace ServiceCommentaire.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitTemplate _rabbitTemplate;

        public CommentController(AppDbContext context, RabbitTemplate rabbitTemplate)
        {
            _context = context;
            _rabbitTemplate = rabbitTemplate;
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            var evt = new CommentCreatedEvent(comment.Id, comment.Text, comment.QualityRating, comment.ValueForMoneyRating, comment.EaseOfUseRating, comment.ProductId);
            _rabbitTemplate.ConvertAndSend("ms.produit", "comment.created", evt);
            return Created(string.Empty, comment);
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
