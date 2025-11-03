using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
