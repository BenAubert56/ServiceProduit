using Steeltoe.Messaging.RabbitMQ.Attributes;
using Steeltoe.Messaging.RabbitMQ.Config;
using ServiceLecture.Events;
using ServiceLecture.Models;

namespace ServiceLecture.Handlers
{
    public class EventHandler
    {
        private readonly AppDbContext _context;
        public EventHandler(AppDbContext context)
        {
            _context = context;
        }

        [DeclareQueue(Name = "ms.produit.created.read-service", Durable = "True")]
        [DeclareQueueBinding(Name = "ms.produit.created.read.binding", QueueName = "ms.produit.created.read-service", ExchangeName = "ms.produit", RoutingKey = "product.created")]
        [RabbitListener(Binding = "ms.produit.created.read.binding")]
        public void OnProductCreated(ProductCreatedEvent e)
        {
            var product = new Product { Id = e.Id, Name = e.Name, Price = e.Price };
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        [DeclareQueue(Name = "ms.comment.created.read-service", Durable = "True")]
        [DeclareQueueBinding(Name = "ms.comment.created.read.binding", QueueName = "ms.comment.created.read-service", ExchangeName = "ms.produit", RoutingKey = "comment.created")]
        [RabbitListener(Binding = "ms.comment.created.read.binding")]
        public void OnCommentCreated(CommentCreatedEvent e)
        {
            var comment = new Comment
            {
                Id = e.Id,
                Text = e.Text,
                QualityRating = e.QualityRating,
                ValueForMoneyRating = e.ValueForMoneyRating,
                EaseOfUseRating = e.EaseOfUseRating,
                ProductId = e.ProductId
            };
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
    }
}
