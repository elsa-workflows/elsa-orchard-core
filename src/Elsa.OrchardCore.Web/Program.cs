using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) =>
{
    logger
        .ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddMediator()
    .AddOrchardCms()
    .AddSetupFeatures("OrchardCore.AutoSetup");

//builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3333")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
//app.UseCors();
app.UseOrchardCore();

app.Run();