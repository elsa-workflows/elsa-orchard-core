using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "Elsa.OrchardCore.Module",
    Name = "Elsa Workflows",
    Author = "Elsa Workflows Contributors",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Visually create workflows with Elsa Workflows.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Module",
    Name = "Elsa Workflow Server Management",
    Description = "Create & manage workflow servers",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.RemoteWorkflowServers",
    Name = "Remote Workflows",
    Description = "Create & manage remote Elsa workflows using a dashboard.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.LocalWorkflowServer",
    Name = "Workflow Server",
    Description = "Turn your Orchard Core tenant into an Elsa Workflow Server and process content, forms and more.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]