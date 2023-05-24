using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "Elsa.OrchardCore.Module",
    Name = "Elsa Workflows",
    Author = "Elsa community",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Create workflows with Elsa.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module",
    Name = "Workflows",
    Description = "Create and manage workflows.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module.Contents",
    Name = "Contents",
    Description = "Provides content activities.",
    Dependencies = new[] {"Elsa.OrchardCore.Module", "OrchardCore.Contents"},
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module.Email",
    Name = "Email",
    Description = "Provides email activities.",
    Dependencies = new[] {"Elsa.OrchardCore.Module", "OrchardCore.Email"},
    Category = "Elsa Workflows"
)]