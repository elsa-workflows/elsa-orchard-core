using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Timers",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Timers",
    Name = "Timer Activities",
    Description = "Provides timer related activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]