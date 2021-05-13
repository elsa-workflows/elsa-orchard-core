using System.ComponentModel.DataAnnotations;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionPropertiesViewModel
    {
        public string Id { get; set; } = default!;
        [Required] public string Name { get; set; } = default!;
        public bool IsSingleton { get; set; }
        public bool DeleteCompletedInstances { get; set; }
        public string? ReturnUrl { get; set; }
        public string ServerId { get; set; } = default!;
    }
}