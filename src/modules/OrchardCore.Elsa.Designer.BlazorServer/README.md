# Elsa.OrchardCore.Designer.BlazorServer

Blazor Server implementation of the Elsa Studio workflow designer for Orchard Core.

## Features

This module provides the Blazor Server-based visual workflow designer interface, allowing users to create, edit, and manage workflows through an interactive graphical interface.

### Visual Workflow Designer

- **Drag-and-Drop Interface** - Intuitive workflow design with drag-and-drop functionality
- **Activity Browser** - Browse and search available workflow activities
- **Visual Workflow Canvas** - Interactive canvas for designing workflow logic
- **Real-time Validation** - Immediate feedback on workflow configuration

### Server-Side Rendering

This implementation uses Blazor Server, which means:

- **Lower Client Requirements** - Works on devices with limited resources
- **Server Processing** - All processing happens on the server
- **Real-time Updates** - SignalR connection for instant UI updates
- **Better Security** - Business logic stays on the server

### Authentication

- Integrated authentication and authorization
- User session management
- Role-based access control for workflow design

## When to Use

Use this module if:

- You want the recommended, production-ready designer experience
- You have reliable network connectivity
- You prefer centralized processing and control
- You want to minimize client-side resource usage

## Dependencies

- `OrchardCore.Elsa.Designer`

## Installation

Enable this feature in the Orchard Core admin dashboard under Features. Note that you should enable either this module OR the BlazorWasm variant, not both.

## Package Information

- **Package ID**: `Elsa.OrchardCore.Designer.BlazorServer`
- **Category**: Elsa
