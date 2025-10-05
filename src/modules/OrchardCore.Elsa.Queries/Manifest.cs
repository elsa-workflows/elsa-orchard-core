using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Queries",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Queries",
    Name = "Query Activities",
    Description = "Provides query related activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa", "OrchardCore.Queries.Sql"]
)]