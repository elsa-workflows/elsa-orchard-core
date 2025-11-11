# Elsa.OrchardCore.Designer.BlazorWasm

Blazor WebAssembly implementation of the Elsa Studio workflow designer for Orchard Core.

## Features

This module provides the Blazor WebAssembly-based visual workflow designer interface, allowing users to create, edit, and manage workflows through an interactive graphical interface that runs entirely in the browser.

### Visual Workflow Designer

- **Drag-and-Drop Interface** - Intuitive workflow design with drag-and-drop functionality
- **Activity Browser** - Browse and search available workflow activities
- **Visual Workflow Canvas** - Interactive canvas for designing workflow logic
- **Real-time Validation** - Immediate feedback on workflow configuration

### Client-Side Rendering

This implementation uses Blazor WebAssembly, which means:

- **Offline Capability** - Can work with cached workflows offline
- **Client Processing** - All UI processing happens in the browser
- **Reduced Server Load** - Less server resource usage for UI operations
- **Lower Latency** - Instant UI interactions without round-trips

### Authentication

- Integrated authentication and authorization
- User session management
- Role-based access control for workflow design

## When to Use

Use this module if:

- You want offline or disconnected workflow design capability
- You want to reduce server load for UI operations
- You have users with high-latency connections
- Your users have modern browsers and sufficient client resources

## Trade-offs

Consider that WebAssembly apps:

- Have larger initial download sizes
- Require modern browser support
- May take longer to load initially

## Dependencies

- `OrchardCore.Elsa.Designer`

## Installation

Enable this feature in the Orchard Core admin dashboard under Features. Note that you should enable either this module OR the BlazorServer variant, not both.

## Package Information

- **Package ID**: `Elsa.OrchardCore.Designer.BlazorWasm`
- **Category**: Elsa
