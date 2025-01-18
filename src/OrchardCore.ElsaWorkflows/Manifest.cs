using OrchardCore.Modules.Manifest;

[assembly: Module(
    Id = "OrchardCore.ElsaWorkflows",
    Name = "Elsa Workflows",
    Author = "Elsa community",
    Website = "https://elsa-workflows.github.io/elsa-core/",
    Version = "1.0.0",
    Description = "Create workflows with Elsa.",
    Category = "Elsa Workflows"
)]

[assembly: Feature(
    Id = "OrchardCore.ElsaWorkflows",
    Name = "Elsa Workflows",
    Description = "Create and manage workflows.",
    Category = "Elsa Workflows"
)]

// [assembly: Feature(
//     Id = "OrchardCore.ElsaWorkflows.Contents",
//     Name = "Contents",
//     Description = "Provides content activities.",
//     Dependencies = new[] {"OrchardCore.ElsaWorkflows", "OrchardCore.Contents"},
//     Category = "Elsa Workflows"
// )]
//
// [assembly: Feature(
//     Id = "OrchardCore.ElsaWorkflows.Email",
//     Name = "Email",
//     Description = "Provides email activities.",
//     Dependencies = new[] {"OrchardCore.ElsaWorkflows", "OrchardCore.Email"},
//     Category = "Elsa Workflows"
// )]