using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot().AddEureka();
builder.Services.AddDiscoveryClient(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

await app.UseOcelot();

app.Run();
