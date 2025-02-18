using OrchardCore.Modules.Manifest;
using OrchardCore.OpenId;

[assembly: Module(
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion,
    Name = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "OrchardCore.ElsaWorkflows",
    Name = "Core Services",
    Description = "Provides the foundational services for Elsa Workflows.",
    Category = "Elsa Workflows",
    Dependencies = [OpenIdConstants.Features.Core]
)]