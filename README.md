# Elsa Workflows for Orchard Core

This repository contains modules that integrate [Elsa Workflows](https://github.com/elsa-workflows/elsa-core) with [Orchard Core](https://github.com/OrchardCMS/OrchardCore).

<img width="1739" alt="image" src="https://github.com/user-attachments/assets/b03c5d71-7473-4067-b04d-a5d245f34d04" />

## Highlights

- Design workflows using Elsa Studio embedded within the Orchard Core admin.
- Execute workflows inside the Orchard Core application for deep framework integration.

## Prerequisites

- .NET SDK 9.0+
- macOS, Windows, or Linux

## Quick start

- Restore and build:

  ```bash
  dotnet --version
  dotnet build
  ```

- Run the sample CMS app:

  ```bash
  # Option A: use the default dev ports
  dotnet run --project src/apps/OrchardCore.Elsa.Web

  # Option B: run on https://localhost:8096 to match the Elsa BaseUrl in appsettings
  ASPNETCORE_URLS="https://localhost:8096" dotnet run --project src/apps/OrchardCore.Elsa.Web
  ```

- Open the site and complete Orchard Core setup:
  - Browse to the running URL (e.g., https://localhost:8096).
  - Choose a database (SQLite is fine for local dev).
  - Pick a recipe (e.g., CMS) and create the admin user.

## Enable Elsa modules

After setup, go to Admin → Configuration → Features and enable the modules you need, for example:

- OrchardCore.Elsa
- OrchardCore.Elsa.UI
- OrchardCore.Elsa.Contents
- OrchardCore.Elsa.Timers
- OrchardCore.Elsa.Queries
- OrchardCore.Elsa.Data

Once enabled, you can find Elsa Studio in the admin and start designing and running workflows.

> First-time note: Enabling the Elsa Workflows module will also enable the OpenID module if it wasn't already active. This can invalidate the current auth session, causing you to be signed out and requiring you to sign back in. If OpenID was already enabled beforehand, you will remain signed in.

## Designer (Blazor)

The Elsa Workflows Designer is built with Blazor. At the moment it runs as a Blazor WebAssembly (WASM) application.

- What this means right now:
  - You need two projects during development:
    - An ASP.NET Core app for Orchard CMS (the server).
    - A Blazor WebAssembly app that hosts the designer shell (the client).
  - The CMS app references the WASM app so the designer’s static assets are served by the CMS.

- How it’s wired in this repo:
  - CMS app: `src/apps/OrchardCore.Elsa.Web`
  - Designer host (WASM): `src/apps/OrchardCore.Elsa.Web.BlazorWasm`
  - Designer module: `src/modules/OrchardCore.Elsa.Designer` (+ `OrchardCore.Elsa.Designer.BlazorWasm`)
  - The CMS project references the WASM project, and is configured to serve its static files and use WebAssembly render mode.

- Future: We plan to support hosting the designer as Blazor Server directly from the Orchard CMS app. When that lands, the extra WASM app won’t be required.

- Customizing the designer:
  - The WASM project is where you customize the UX for your activities.
  - You can control icons, colors, and provide custom components or hook into other extensibility points.
  - Example: see `src/modules/OrchardCore.Elsa.Designer/ActivityIconProvider.cs` for per-activity icons/colors.
  - When you add a custom activity on the server, add corresponding UI in the client to tailor how it appears and behaves in the designer.

## Solutions

- Open `Elsa.OrchardCore.sln` to work with the Orchard Core modules and sample app.

## Upstream Elsa resources

- Documentation: https://docs.elsaworkflows.io/
- Repository: https://github.com/elsa-workflows/elsa-core

## Contributing and license

Contributions are welcome! See the LICENSE file for details.
