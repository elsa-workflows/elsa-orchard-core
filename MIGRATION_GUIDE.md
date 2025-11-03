# Migration Guide: Upgrading to OrchardCore.Elsa 3.6.0+

## Overview

Starting with version 3.6.0, the OrchardCore.Elsa Designer architecture has changed. The Designer is now a Razor Class Library (RCL) instead of a bundled Blazor WebAssembly application.

## Breaking Changes

**The bundled WASM scenario has been removed.** You must now:
1. Create your own Blazor WebAssembly project
2. Reference the OrchardCore.Elsa.Designer RCL
3. Reference your WASM project from your web application

## Migration Steps

### Step 1: Create a Blazor WebAssembly Project

Create a new Blazor WebAssembly project in your solution:

```bash
# Navigate to your apps or client projects folder
cd src/apps

# Create new Blazor WASM project
dotnet new blazorwasm -n YourProject.Blazor
```

### Step 2: Add NuGet Package Reference

Update your new WASM project to reference the Designer RCL:

**YourProject.Blazor.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" />
        <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" PrivateAssets="all" />
        <PackageReference Include="OrchardCore.Elsa.Designer" Version="3.6.0" />
    </ItemGroup>
</Project>
```

### Step 3: Configure Program.cs

**YourProject.Blazor/Program.cs:**
```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrchardCore.Elsa.Designer.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddElsaDesigner();

var app = builder.Build();

await app.RunStartupTasksAsync();
await app.RunAsync();
```

The `AddElsaDesigner()` extension method handles both registering root components and configuring all required services.

### Step 4: Add Configuration File

**YourProject.Blazor/wwwroot/appsettings.json:**
```json
{
  "Backend": {
    "Url": ""
  }
}
```

> **Note:** Leave the `Url` empty if your WASM app is served from the same origin as your web application.

### Step 5: Update Your Web Application Project

Add a reference to your new WASM project:

**YourProject.Web.csproj:**
```xml
<ItemGroup>
    <ProjectReference Include="..\YourProject.Blazor\YourProject.Blazor.csproj" />
</ItemGroup>
```

### Step 6: Remove Old Configuration (if present)

If you previously configured `ElsaBlazorOptions`, you can remove it as it's no longer used:

```csharp
// REMOVE THIS - No longer needed
services.Configure<ElsaBlazorOptions>(options =>
{
    options.UseHostBlazorApp = true;
});
```

### Step 7: Verify Your Views

The OrchardCore.Elsa module views should already be using the simplified script reference:

```html
<!-- This should already be in place -->
<script src="/_framework/blazor.webassembly.js"></script>
```

## Benefits of This Change

1. **More Control:** You have full control over your WASM project configuration
2. **Better Separation:** Clear separation between RCL components and WASM hosting
3. **Flexibility:** Easier to customize and extend the Designer for your needs
4. **Standard Pattern:** Follows Blazor best practices and conventions

## Troubleshooting

### Issue: 404 on /_framework/blazor.webassembly.js

**Solution:** Ensure your WASM project is referenced by your web project. The reference automatically sets up the middleware to serve WASM files.

### Issue: 404 on /_framework/dotnet.js or other _framework resources

**Problem:** Blazor tries to load resources from the wrong path (e.g., `/Admin/ElsaWorkflows/WorkflowDefinitions/Edit/_framework/dotnet.js` instead of `/_framework/dotnet.js`)

**Solution:** The OrchardCore.Elsa module views configure Blazor with `autostart="false"` and a `loadBootResource` callback to ensure all `_framework` resources load from the root path. If you've customized the views, make sure you include this configuration:

```html
<script src="/_framework/blazor.webassembly.js" autostart="false"></script>
<script at="Foot">
    document.addEventListener('DOMContentLoaded', function () {
        Blazor.start({
            loadBootResource: function (type, name, defaultUri, integrity) {
                // Ensure all _framework resources are loaded from the root
                if (defaultUri.startsWith('_framework/')) {
                    return `/${defaultUri}`;
                }
                return defaultUri;
            }
        });
    });
</script>
```

### Issue: Components Not Rendering

**Solution:** Make sure you're calling `builder.AddElsaDesigner()` or both:
- `builder.RootComponents.RegisterElsaDesignerComponents()`
- `builder.Services.AddElsaDesigner(builder.Configuration)`

### Issue: Backend Connection Errors

**Solution:** Verify your `appsettings.json` in the WASM project has the correct backend URL (or empty string for same-origin).

## Example Project Structure

After migration, your solution should look like this:

```
YourSolution/
├── src/
│   ├── YourProject.Web/           # ASP.NET Core + OrchardCore
│   │   └── YourProject.Web.csproj # References YourProject.Blazor
│   └── YourProject.Blazor/        # Blazor WASM
│       ├── Program.cs             # Uses Designer extensions
│       ├── wwwroot/
│       │   └── appsettings.json
│       └── YourProject.Blazor.csproj # References OrchardCore.Elsa.Designer
```

## Reference Implementation

See `OrchardCore.Elsa.Web.Blazor` in the source repository for a complete reference implementation.

## Need Help?

If you encounter issues during migration, please:
1. Check the reference implementation in `src/apps/OrchardCore.Elsa.Web.Blazor`
2. Review the README in the OrchardCore.Elsa.Web.Blazor project
3. Open an issue on the GitHub repository with details about your setup
