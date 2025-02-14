using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "OrchardCore.ElsaWorkflows.Content",
    Name = "Elsa Workflows Content Activities",
    Author = "Orchard Community",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Provides content activities for Elsa Workflows.",
    Category = "Elsa Workflows",
    Dependencies = ["OrchardCore.ElsaWorkflows", "OrchardCore.Contents"]
)]

[assembly: Feature(
    Id = "OrchardCore.ElsaWorkflows.Content",
    Name = "Elsa Workflows Content Activities",
    Description = "Provides content activities for Elsa Workflows.",
    Category = "Elsa Workflows",
    Dependencies = ["OrchardCore.ElsaWorkflows", "OrchardCore.Contents"]
)]