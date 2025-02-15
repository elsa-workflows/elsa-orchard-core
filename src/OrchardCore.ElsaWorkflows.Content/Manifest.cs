using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Workflows Content Activities",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.ElsaWorkflows.Content",
    Name = "Elsa Workflows Content Activities",
    Description = "Provides content activities.",
    Category = "Elsa Workflows",
    Dependencies = ["OrchardCore.ElsaWorkflows", "OrchardCore.Contents"]
)]