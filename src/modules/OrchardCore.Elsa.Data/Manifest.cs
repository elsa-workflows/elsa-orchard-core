using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Data",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Data.Csv",
    Name = "CSV Activities",
    Description = "Provides CSV related activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]