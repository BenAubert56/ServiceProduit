using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Connector.RabbitMQ;

namespace ServiceLecture
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddDiscoveryClient(builder.Configuration);

            builder.Services.AddDbContext<Models.AppDbContext>(opt =>
                opt.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

            builder.Services.AddRabbitMQConnection(builder.Configuration);
            builder.Services.AddRabbitServices(true);
            builder.Services.AddRabbitAdmin();
            builder.Services.AddRabbitTemplate();
            builder.Services.AddRabbitExchange("ms.produit", ExchangeType.TOPIC);
            builder.Services.AddScoped<Handlers.EventHandler>();
            builder.Services.AddRabbitListeners<Handlers.EventHandler>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
