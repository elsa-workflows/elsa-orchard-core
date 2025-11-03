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
    Name = "Elsa Workflows",
    Description = "Provides foundational Elsa Workflows services.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Contents", OpenIdConstants.Features.Core]
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Http",
    Name = "HTTP Activities",
    Description = "Provides HTTP activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]