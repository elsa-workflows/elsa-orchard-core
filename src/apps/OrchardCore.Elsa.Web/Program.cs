using OrchardCore.Elsa.Extensions;
using OrchardCore.Elsa.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
{
    logger
        .ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddOrchardCms();

builder.Services.ConfigureWebAssemblyStaticFiles();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.RewriteElsaStudioWebAssemblyAssets();
app.UseStaticFiles();
app.UseOrchardCore();

app.Run();