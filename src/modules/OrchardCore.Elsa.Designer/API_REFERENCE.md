# OrchardCore.Elsa.Designer API Reference

## Overview

The OrchardCore.Elsa.Designer is a Razor Class Library (RCL) that provides Blazor components and services for the Elsa workflow designer. The core library is hosting-model agnostic and is accompanied by two lightweight wrapper packages:

- `OrchardCore.Elsa.Designer.BlazorWasm` – extension methods and references for WebAssembly hosts.
- `OrchardCore.Elsa.Designer.BlazorServer` – extension methods and references for Blazor Server hosts.

Use the package that matches your hosting model, or build a custom host by calling the shared `AddElsaDesignerCore` method described below.

## Extension Methods

### WebAssemblyHostBuilder Extensions

Located in: `OrchardCore.Elsa.Designer.Extensions.WebAssemblyHostBuilderExtensions`

#### `AddElsaDesigner()`

The primary fluent API for configuring the Elsa Designer.

```csharp
public static WebAssemblyHostBuilder AddElsaDesigner(
    this WebAssemblyHostBuilder builder)
```

**Description:**
Registers all Elsa Designer root components and configures all required services in a single call.

**Usage:**
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddElsaDesigner();
```

**What it does:**
1. Calls `RegisterElsaDesignerComponents()` on `builder.RootComponents`
2. Calls `AddElsaDesigner(builder.Configuration)` on `builder.Services`

---

### Service Collection Extensions

Located in: `OrchardCore.Elsa.Designer.Extensions.ServiceCollectionExtensions`

#### `AddElsaDesignerCore(IConfiguration, Action<IServiceCollection>)`

Configures all shared Elsa Designer services. The platform-specific packages invoke this method with the appropriate callbacks to register WebAssembly or Server implementations.

```csharp
public static IServiceCollection AddElsaDesignerCore(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<IServiceCollection> configurePlatformServices)
```

**Description:**
Registers all services required for the Elsa Designer, including:
- Backend service for API communication
- Elsa Studio core services (via `configurePlatformServices`)
- Workflow management services
- Authentication provider (no-op for embedded scenarios)
- Activity display settings

**Parameters:**
- `configuration`: The configuration to bind backend options from (looks for `Backend:Url` setting)
- `configurePlatformServices`: Callback that registers hosting-model specific services (e.g. calling `AddCore()` from the Blazor Server or WebAssembly packages).

**Usage:**
```csharp
// WebAssembly host
builder.Services.AddElsaDesigner(builder.Configuration); // Provided by OrchardCore.Elsa.Designer.BlazorWasm

// Blazor Server host
builder.Services.AddElsaDesigner(builder.Configuration); // Provided by OrchardCore.Elsa.Designer.BlazorServer
```

**When to use:**
Call `AddElsaDesignerCore` when you need to build a custom hosting wrapper. Otherwise, reference the platform-specific package and invoke its `AddElsaDesigner` extension method.

---

#### `RegisterElsaDesignerComponents()`

Registers Elsa Designer root components.

```csharp
public static RootComponentMappingCollection RegisterElsaDesignerComponents(
    this RootComponentMappingCollection rootComponents)
```

**Description:**
Registers the custom Elsa Studio elements that render in the DOM.

**Usage:**
```csharp
builder.RootComponents.RegisterElsaDesignerComponents();
```

**When to use:**
Use this directly if you're calling `AddElsaDesigner(IConfiguration)` manually instead of using the `WebAssemblyHostBuilder.AddElsaDesigner()` extension.

---

### WebAssemblyHost Extensions

Located in: `OrchardCore.Elsa.Designer.Extensions.WebAssemblyHostExtensions`

#### `RunStartupTasksAsync()`

Executes all registered startup tasks.

```csharp
public static async Task RunStartupTasksAsync(
    this WebAssemblyHost host,
    CancellationToken cancellationToken = default)
```

**Description:**
Retrieves all registered `IStartupTask` services and executes them sequentially.

**Parameters:**
- `cancellationToken`: Optional cancellation token (default: `default`)

**Usage:**
```csharp
var app = builder.Build();
await app.RunStartupTasksAsync();
await app.RunAsync();
```

---

## Configuration

### Backend Configuration

The Designer requires backend configuration in `wwwroot/appsettings.json`:

```json
{
  "Backend": {
    "Url": ""
  }
}
```

**Backend URL Options:**

| Value | Description |
|-------|-------------|
| `""` (empty) | Use same origin as the WASM app (recommended for integrated scenarios) |
| `"http://localhost:5000"` | Use specific backend URL (for standalone development) |
| `"/api"` | Use relative path on the same origin |

---

## Typical Usage Patterns

### Pattern 1: Simple Integration (Recommended)

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

### Pattern 2: Manual Configuration (Advanced)

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register components
builder.RootComponents.RegisterElsaDesignerComponents();

// Configure services with custom configuration
builder.Services.AddElsaDesigner(builder.Configuration);

// Add your custom services here
builder.Services.AddSingleton<IMyCustomService, MyCustomService>();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

### Pattern 3: Multiple Configuration Sources

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add additional configuration sources
builder.Configuration.AddJsonFile("custom-config.json", optional: true);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

---

## Components

The Designer provides several Blazor components that are automatically registered:

### Available Components

| Component | Purpose |
|-----------|---------|
| `WorkflowEditorOrInstanceWrapper` | Main workflow editor/viewer component |
| `WorkflowDefinitionEditorWrapper` | Workflow definition editor |
| `WorkflowDefinitionListWrapper` | List of workflow definitions |
| `WorkflowInstanceListWrapper` | List of workflow instances |
| `WorkflowInstanceViewerWrapper` | Workflow instance viewer |
| `BackendProvider` | Provides backend context |
| `ThemedComponentWrapper` | Provides theming context |

These components are registered as custom elements and rendered in your Razor views via the OrchardCore.Elsa module.

---

## Dependencies

The Designer has the following package dependencies:

- `Microsoft.AspNetCore.Components.WebAssembly`
- `Elsa.Studio.Core.BlazorWasm`
- `Elsa.Studio.Login.BlazorWasm`
- `Elsa.Studio.Workflows.Core`
- `Elsa.Studio.Workflows`
- `Elsa.Studio.Shell`

All dependencies are automatically included when you reference the `OrchardCore.Elsa.Designer` package.

---

## Authentication

The Designer includes a `NoopAuthenticationProviderManager` which returns null for authentication tokens. This is appropriate for embedded scenarios where authentication is handled by the host ASP.NET Core application.

If you need custom authentication:

```csharp
builder.Services.AddElsaDesigner(builder.Configuration);

// Replace with your custom authentication provider
builder.Services.Replace(ServiceDescriptor.Scoped<IAuthenticationProviderManager, MyCustomAuthProvider>());
```

---

## Troubleshooting

### Components Not Rendering

**Issue:** Workflow editor components don't render.

**Solution:** Ensure you've called `builder.AddElsaDesigner()` or both `RegisterElsaDesignerComponents()` and `AddElsaDesigner(configuration)`.

### Backend Connection Errors

**Issue:** Cannot connect to backend API.

**Solution:** Check your `appsettings.json` configuration. For same-origin scenarios, use an empty string `""` for the URL.

### Startup Tasks Not Running

**Issue:** Application behavior is incorrect due to missing initialization.

**Solution:** Make sure you're calling `await app.RunStartupTasksAsync()` before `await app.RunAsync()`.

---

## Version History

### v3.6.0
- Initial release as Razor Class Library
- Added fluent extension methods for easy integration
- Separated from standalone WASM application
