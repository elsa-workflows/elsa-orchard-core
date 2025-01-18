using System.ComponentModel.DataAnnotations;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Create;

public class CreateViewModel
{
    [Required]
    public string Name { get; set; }
}
