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
    Name = "Elsa Workflows",
    Description = "Create workflow with Elsa.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.RemoteWorkflowServers",
    Name = "Remote Workflows",
    Description = "Create & manage Elsa workflows on remote servers.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.LocalWorkflowServer",
    Name = "Workflow Server",
    Description = "Create & manage Elsa workflows local to this tenant.",
    Dependencies = new[] {"Elsa.OrchardCore.Module"},
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "Elsa.OrchardCore.Email",
    Name = "Email",
    Description = "Provides email activities.",
    Dependencies = new[] {"Elsa.OrchardCore.LocalWorkflowServer", "OrchardCore.Email"},
    Category = "Elsa Workflows"
)]