using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Elsa Timers",
    Author = ManifestConstants.OrchardCoreTeam,
    Website = ManifestConstants.OrchardCoreWebsite,
    Version = ManifestConstants.OrchardCoreVersion
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Timers",
    Name = "Timer Services and Activities",
    Description = "Provides common timer services and activities.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa"]
)]

[assembly: Feature(
    Id = "OrchardCore.Elsa.Timers.Quartz",
    Name = "Quartz Timer Provider",
    Description = "Provides Quartz-based timer services. Suitable for clustered deployments.",
    Category = "Elsa",
    Dependencies = ["OrchardCore.Elsa.Timers"]
)]