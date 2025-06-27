using ServiceLecture.Events;
using ServiceLecture.Models;
using Steeltoe.Messaging.RabbitMQ.Attributes;

namespace ServiceLecture.Handlers
{
    public class EventHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [DeclareQueue(Name = "ms.produit.created.read-service", Durable = "True")]
        [DeclareQueueBinding(Name = "ms.produit.created.read.binding", QueueName = "ms.produit.created.read-service", ExchangeName = "ms.produit", RoutingKey = "product.created")]
        [RabbitListener(Binding = "ms.produit.created.read.binding")]
        public void OnProductCreated(ProductCreatedEvent e)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var product = new Product { Id = e.Id, Name = e.Name, Price = e.Price };
            context.Products.Add(product);
            context.SaveChanges();
        }

        [DeclareQueue(Name = "ms.comment.created.read-service", Durable = "True")]
        [DeclareQueueBinding(Name = "ms.comment.created.read.binding", QueueName = "ms.comment.created.read-service", ExchangeName = "ms.produit", RoutingKey = "comment.created")]
        [RabbitListener(Binding = "ms.comment.created.read.binding")]
        public void OnCommentCreated(CommentCreatedEvent e)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var comment = new Comment
            {
                Id = e.Id,
                Text = e.Text,
                QualityRating = e.QualityRating,
                ValueForMoneyRating = e.ValueForMoneyRating,
                EaseOfUseRating = e.EaseOfUseRating,
                ProductId = e.ProductId
            };
            context.Comments.Add(comment);
            context.SaveChanges();
        }
    }
}
