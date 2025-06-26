
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using Steeltoe.Common.Http.Discovery;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using Steeltoe.Messaging.RabbitMQ.Config;
using RabbitMQ.Client;

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
            builder.Services.AddHttpClient("service-commentaire", client =>
            {
                client.BaseAddress = new Uri("lb://service-commentaire/");
            })
            .AddRandomLoadBalancer()
            .AddTransientHttpErrorPolicy(policy =>
                policy.CircuitBreakerAsync(3, TimeSpan.FromSeconds(5)))
            .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 5,
                maxQueuingActions: int.MaxValue));

            builder.Services.AddDbContext<Models.AppDbContext>(opt =>
                opt.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

            builder.Services.AddRabbitMQConnection(builder.Configuration);
            builder.Services.AddRabbitServices(true);
            builder.Services.AddRabbitAdmin();
            builder.Services.AddRabbitTemplate();
            builder.Services.AddRabbitExchange("ms.produit", ExchangeType.TOPIC);

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
