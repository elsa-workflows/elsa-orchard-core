using System;
using Elsa.Client.Models;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionViewModel
    {
        public WorkflowDefinition WorkflowDefinition { get; set; } = default!;
        public string ServerId { get; set; } = default!;
        public Uri ServerUrl { get; set; } = default!;
    }
}