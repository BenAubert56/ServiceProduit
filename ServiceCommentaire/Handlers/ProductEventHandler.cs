using Steeltoe.Messaging.RabbitMQ.Attributes;
using Steeltoe.Messaging.RabbitMQ.Config;
using ServiceCommentaire.Events;
using System;

namespace ServiceCommentaire.Handlers
{
    public class ProductEventHandler
    {
        [DeclareQueue(Name = "ms.produit.created.comment-service", Durable = "True")]
        [DeclareQueueBinding(Name = "ms.produit.created.binding", QueueName = "ms.produit.created.comment-service", ExchangeName = "ms.produit", RoutingKey = "product.created")]
        [RabbitListener(Binding = "ms.produit.created.binding")]
        public void OnProductCreated(ProductCreatedEvent e)
        {
            Console.WriteLine($"Received product created: {e.Name}");
        }
    }
}
