using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.ElsaWorkflows.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
{
    logger
        .ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddOrchardCms();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Insert this BEFORE UseStaticFiles or Orchard's static file pipeline to allow rewriting of wasm asset requests.
app.UseElsaStudioBlazorAssetsUrlRewriter();
app.UseStaticFiles();
app.UseOrchardCore();

app.Run();