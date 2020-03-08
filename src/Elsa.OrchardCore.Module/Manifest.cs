using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "Elsa.OrchardCore.Module",
    Name = "Workflows",
    Author = "The Elsa Team",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Visually create workflows with Elsa",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module",
    Name = "Workflows",
    Description = "Create & manage workflows using Elsa",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Server",
    Name = "Workflow Server",
    Description = "Turn your Orchard Core tenant into an Elsa Workflow Server and process content, forms and more.",
    Dependencies = new[] { "Elsa.OrchardCore.Module" },
    Category = "Elsa Workflows"
)]