using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Timers",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Timers.Local",
    Name = "Local Timer Activities",
    Description = "Provides timer related activities using local scheduling. Not suitable for clustered hosting.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Timers.Orchard",
    Name = "Orchard Timer Activities",
    Description = "Provides timer related activities using Orchard's background scheduling. Not suitable for high-frequency timers below 1 minute.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]