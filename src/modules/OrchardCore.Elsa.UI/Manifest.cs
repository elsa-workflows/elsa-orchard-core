using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa UI",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.UI",
    Name = "UI Activities",
    Description = "Provides UI related activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]