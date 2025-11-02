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
    Name = "Elsa",
    Description = "Provides foundational Elsa services.",
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