using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Contents",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Contents",
    Name = "Content Activities",
    Description = "Provides content related activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa", "OrchardCore.Contents", "OrchardCore.Title", "OrchardCore.Taxonomies"]
)]