using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Workflows Contents",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.ElsaWorkflows.Contents",
    Name = "Content Activities",
    Description = "Provides content related activities.",
    Category = "Elsa Workflows",
    Dependencies = ["OrchardCore.ElsaWorkflows", "OrchardCore.Contents"]
)]