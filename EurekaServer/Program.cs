using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.EurekaServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery(o => o.UseEurekaServer());

var app = builder.Build();

app.Run();
