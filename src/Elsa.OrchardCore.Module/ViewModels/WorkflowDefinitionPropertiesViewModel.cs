using System.ComponentModel.DataAnnotations;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowDefinitionPropertiesViewModel
    {
        public string? DefinitionId { get; set; } 

        [Required] public string Name { get; set; } = default!;
        
        public string? ReturnUrl { get; set; }
    }
}
