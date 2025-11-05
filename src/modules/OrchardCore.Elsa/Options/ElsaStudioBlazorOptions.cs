using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrchardCore.Elsa.Options;

/// <summary>
/// Options controlling how the Elsa Studio designer components are hosted.
/// </summary>
public class ElsaStudioBlazorOptions
{
    /// <summary>
    /// Gets or sets the render mode used by the embedded designer components.
    /// </summary>
    public RenderMode RenderMode { get; set; } = RenderMode.WebAssembly;

    /// <summary>
    /// Gets a value indicating whether the designer should run as a Blazor WebAssembly application.
    /// </summary>
    public bool IsWebAssembly => RenderMode is RenderMode.WebAssembly or RenderMode.WebAssemblyPrerendered;

    /// <summary>
    /// Gets a value indicating whether the designer should run using Blazor Server.
    /// </summary>
    public bool IsServer => RenderMode is RenderMode.Server or RenderMode.ServerPrerendered;
}
