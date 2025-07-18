
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Config;
using RabbitMQ.Client;
using Steeltoe.Connector.RabbitMQ;

namespace ServiceProduit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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
            builder.Services.AddRabbitExchange("ms.produit", Steeltoe.Messaging.RabbitMQ.Config.ExchangeType.TOPIC);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
