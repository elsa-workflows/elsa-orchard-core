using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion,
    Name = "Elsa Agents"
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Agents",
    Name = "Elsa Agents",
    Description = "Provides UI and persistence for Elsa agents.",
    Category = "Elsa",
    Dependencies = [
        "OrchardCore.Contents",
        "OrchardCore.ContentTypes",
        "OrchardCore.ContentFields",
        "OrchardCore.Settings",
        "OrchardCore.ResourceManagement",
        "OrchardCore.Navigation"
    ]
)]
