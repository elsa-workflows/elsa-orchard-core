using OrchardCore.Modules.Manifest;
using OrchardCore.OpenId;

[assembly: Module(
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion,
    Name = "Elsa"
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa",
    Name = "Core Elsa Services",
    Description = "Provides the foundational services for Elsa.",
    Category = "Elsa",
    Dependencies = [OpenIdConstants.Features.Core]
)]