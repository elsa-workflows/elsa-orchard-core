# Refactoring Summary: OrchardCore.Elsa Designer Architecture

## Overview

The OrchardCore.Elsa Designer has been refactored from a standalone Blazor WebAssembly application into a Razor Class Library (RCL) with a separate reference WASM application.

## Changes Made

### 1. OrchardCore.Elsa.Designer (Converted to RCL)

**Before:** Standalone Blazor WASM application (`Microsoft.NET.Sdk.BlazorWebAssembly`)
**After:** Razor Class Library (`Microsoft.NET.Sdk.Razor`)

**Changes:**
- Updated project SDK from `Microsoft.NET.Sdk.BlazorWebAssembly` to `Microsoft.NET.Sdk.Razor`
- Removed `Microsoft.AspNetCore.Components.WebAssembly.DevServer` package reference
- Removed `StaticWebAssetBasePath` property
- Removed standalone application files:
  - `Program.cs`
  - `appsettings.json`
  - `appsettings.Development.json`
  - `wwwroot/appsettings.json`
  - `Properties/` folder
- Created `Extensions/ServiceCollectionExtensions.cs` with helper methods:
  - `AddElsaDesigner(IConfiguration)` - Configures all Elsa Designer services
  - `RegisterElsaDesignerComponents()` - Registers root components
  - `NoopAuthenticationProviderManager` - Authentication provider for embedded scenarios
- Created `Extensions/WebAssemblyHostExtensions.cs`:
  - `RunStartupTasksAsync()` - Convenient method to execute all registered startup tasks
- Created `Extensions/WebAssemblyHostBuilderExtensions.cs`:
  - `AddElsaDesigner()` - Fluent API that registers components and services in one call

### 2. OrchardCore.Elsa.Web.Blazor (New Project)

**Purpose:** Reference Blazor WebAssembly application and template for consumers

**Created Files:**
- `OrchardCore.Elsa.Web.Blazor.csproj` - Blazor WASM project file
- `Program.cs` - Application entry point using Designer extensions
- `_Imports.razor` - Razor imports
- `wwwroot/appsettings.json` - Configuration file
- `Properties/launchSettings.json` - Launch configuration
- `README.md` - Comprehensive documentation for consumers

### 3. OrchardCore.Elsa Module (Simplified)

**Removed:**
- `Options/ElsaBlazorOptions.cs` - No longer needed
- Project reference to `OrchardCore.Elsa.Designer`
- Options registration in `CoreStartup.cs`

**Updated:**
- `Features/CoreStartup.cs`:
  - Removed `using OrchardCore.Elsa.Options`
  - Removed `services.AddOptions<ElsaBlazorOptions>()`
  - Cleaned up service registration
- `Views/WorkflowDefinitions/Edit.cshtml`:
  - Removed options injection
  - Simplified to use host-provided WASM runtime
  - Changed from `<component>` tag to `<div>` with data attributes
  - Configures Blazor with `autostart="false"` and `loadBootResource` callback to ensure `_framework` resources load from root path (fixes issue with admin area relative paths)
- `Views/WorkflowInstances/Details.cshtml`:
  - Same simplifications as Edit.cshtml

### 4. OrchardCore.Elsa.Web (Updated)

**Changes:**
- Added project reference to `OrchardCore.Elsa.Web.Blazor`
- Now serves the WASM application automatically via project reference

### 5. Solution Structure

**Before:**
```
src/
  modules/
    OrchardCore.Elsa.Designer/ (WASM App)
    OrchardCore.Elsa/ (Module, references Designer)
  apps/
    OrchardCore.Elsa.Web/ (Web App)
```

**After:**
```
src/
  modules/
    OrchardCore.Elsa.Designer/ (RCL)
    OrchardCore.Elsa/ (Module, no Designer reference)
  apps/
    OrchardCore.Elsa.Web/ (Web App, references Blazor)
    OrchardCore.Elsa.Web.Blazor/ (WASM App, references Designer RCL)
```

## Benefits

### For Package Consumers

1. **More Flexibility:** Consumers create their own WASM project, giving them full control
2. **Simpler Integration:** Clear separation between RCL and WASM concerns
3. **Better Documentation:** README provides step-by-step instructions
4. **Extension Methods:** Easy-to-use `AddElsaDesigner()` and `RegisterElsaDesignerComponents()` methods

### For the Project

1. **Cleaner Architecture:** RCL focuses on components, WASM app on hosting
2. **Reference Implementation:** OrchardCore.Elsa.Web.Blazor serves as example
3. **Simplified Module:** OrchardCore.Elsa no longer needs Designer reference
4. **Better Separation of Concerns:** Each project has a clear, single purpose

## How Consumers Should Use It

### Step 1: Create WASM Project
```bash
dotnet new blazorwasm -n YourProject.Blazor
```

### Step 2: Reference OrchardCore.Elsa.Designer NuGet Package
```xml
<PackageReference Include="OrchardCore.Elsa.Designer" Version="3.6.0" />
```

### Step 3: Configure Program.cs
```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

### Step 4: Reference WASM from Web Project
```xml
<ProjectReference Include="..\YourProject.Blazor\YourProject.Blazor.csproj" />
```

### Step 5: Enable OrchardCore.Elsa Module
The module's views automatically load `/_framework/blazor.webassembly.js` from the host.

## Build & Packaging

- All projects build successfully with existing CI/CD
- OrchardCore.Elsa.Designer is packable (RCL)
- OrchardCore.Elsa.Web.Blazor is NOT packable (reference app only)
- GitHub Actions workflow unchanged - automatically detects and packs all packable projects

## Migration Notes

**Breaking Change:** The bundled WASM scenario has been removed. All consumers must now:
1. Create their own Blazor WASM project
2. Reference OrchardCore.Elsa.Designer RCL
3. Reference their WASM project from their web application

This change provides better flexibility and follows Blazor best practices.
