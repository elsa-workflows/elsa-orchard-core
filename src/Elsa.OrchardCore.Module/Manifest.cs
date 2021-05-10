using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "Elsa.OrchardCore.Module",
    Name = "Workflows",
    Author = "Elsa Workflows Contributors",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Visually create workflows with Elsa Workflows.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module",
    Name = "Workflow Server Management",
    Description = "Create & manage workflow servers",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Dashboard",
    Name = "Workflow Dashboard",
    Description = "Create & manage workflows using a dashboard.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Server",
    Name = "Workflow Server",
    Description = "Turn your Orchard Core tenant into a Workflow Server and process content, forms and more.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]