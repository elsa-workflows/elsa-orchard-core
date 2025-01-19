using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
    .AddOrchardCms();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Insert this BEFORE UseStaticFiles or Orchard's static file pipeline.
        app.Use(async (context, next) =>
        {
            // 1) Check if the request path itself contains /Admin/ElsaWorkflows/.../_content/...
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.Contains("/Admin/ElsaWorkflows", StringComparison.OrdinalIgnoreCase)
                && path.Contains("/_content/", StringComparison.OrdinalIgnoreCase))
            {
                // Example: /Admin/ElsaWorkflows/WorkflowDefinitions/Edit/XYZ/_content/Elsa.Studio.DomInterop/dom.entry.js
                // We want to rewrite to /OrchardCore.ElsaWorkflows/_content/Elsa.Studio.DomInterop/dom.entry.js
                var index = path.IndexOf("/_content/", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    var resourcePath = path.Substring(index); // e.g. "/_content/Elsa.Studio.DomInterop/dom.entry.js"
                    var newPath = "/OrchardCore.ElsaWorkflows" + resourcePath;
                    context.Request.Path = newPath;

                    // Optionally clear the query string if you don't want it carried forward.
                    context.Request.QueryString = QueryString.Empty;
                }
            }
            else
            {
                // 2) Otherwise, check if there's a `returnUrl` query parameter containing /_content/...
                //    e.g. "?returnUrl=/Admin/ElsaWorkflows/WorkflowDefinitions/Edit/XYZ/_content/..."
                if (context.Request.Query.TryGetValue("returnUrl", out var returnUrlValues))
                {
                    var returnUrl = returnUrlValues.ToString();
                    var index = returnUrl.IndexOf("/_content/", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        var resourcePath = returnUrl.Substring(index); // e.g. "/_content/Microsoft.AspNetCore.Components.CustomElements/..."
                        var newPath = "/OrchardCore.ElsaWorkflows" + resourcePath;
                        context.Request.Path = newPath;
                        
                        // Remove the query string so the static file middleware sees a clean path.
                        context.Request.QueryString = QueryString.Empty;
                    }
                }
            }

            await next();
        });

app.UseStaticFiles();
app.UseOrchardCore();

app.Run();