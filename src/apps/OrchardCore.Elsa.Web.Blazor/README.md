# OrchardCore.Elsa.Web.Blazor

This is a reference Blazor WebAssembly application that demonstrates how to set up the Elsa workflow designer in your own OrchardCore application.

## Purpose

This project serves as:
- A **workbench/reference application** for developing and testing the Elsa Designer
- A **template** showing consumers how to create their own Blazor WASM project for Elsa workflows

## Architecture

The Elsa Designer is split into two projects:
- **OrchardCore.Elsa.Designer** (RCL) - Contains all Blazor components, services, and extensions
- **OrchardCore.Elsa.Web.Blazor** (WASM App) - Standalone Blazor WebAssembly application that references the Designer RCL

## Creating Your Own WASM Project

To use Elsa workflows in your OrchardCore application, follow these steps:

### 1. Create a Blazor WebAssembly Project

```bash
dotnet new blazorwasm -n YourProject.Blazor
```

### 2. Add Reference to OrchardCore.Elsa.Designer

In your `YourProject.Blazor.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="OrchardCore.Elsa.Designer" Version="3.6.0" />
</ItemGroup>
```

### 3. Configure Program.cs

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

That's it! The `AddElsaDesigner()` extension method registers root components and configures all required services.

### 4. Configure Backend URL

In `wwwroot/appsettings.json`:

```json
{
  "Backend": {
    "Url": ""
  }
}
```

Leave the URL empty if the WASM app is served from the same origin as your OrchardCore application.

### 5. Reference WASM Project from Your Web Application

In your `YourProject.Web.csproj`:

```xml
<ItemGroup>
    <ProjectReference Include="..\YourProject.Blazor\YourProject.Blazor.csproj" />
</ItemGroup>
```

### 6. Enable OrchardCore.Elsa Module

Make sure to enable the `OrchardCore.Elsa` module in your OrchardCore application. The module's views already reference `/_framework/blazor.webassembly.js`, which will be served by your WASM project.

## How It Works

- The OrchardCore.Elsa module provides views that load the Blazor WASM runtime
- Your WASM project provides the `_framework` folder and all necessary Blazor assets
- The Designer RCL provides all the Elsa workflow components and services
- The ASP.NET Core application serves both the server-side content and the WASM application

## Extension Methods

The OrchardCore.Elsa.Designer package provides convenient extension methods:

### WebAssemblyHostBuilder Extensions
- **`AddElsaDesigner()`** - Fluent API that registers root components and configures services in one call

### Service Collection Extensions (for advanced scenarios)
- **`AddElsaDesigner(IConfiguration)`** - Configures all Elsa Designer services
- **`RegisterElsaDesignerComponents()`** - Registers root components only

### WebAssemblyHost Extensions
- **`RunStartupTasksAsync()`** - Executes all registered startup tasks

## Development

Run the OrchardCore.Elsa.Web application alongside this WASM app:

```bash
# Terminal 1 - Run the OrchardCore web app
cd src/apps/OrchardCore.Elsa.Web
dotnet run

# The WASM app is automatically served via the web app's reference
```
