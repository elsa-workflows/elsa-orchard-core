﻿namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionListEntry
    {
        public string Id { get; set; } = default!;
        public string DefinitionId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int WorkflowInstanceCount { get; set; }
        public int? PublishedVersion { get; set; }
        public int LatestVersion { get; set; }
        public bool IsChecked { get; set; }
    }
}