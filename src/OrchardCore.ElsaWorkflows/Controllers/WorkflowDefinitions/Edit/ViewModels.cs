using System.ComponentModel.DataAnnotations;

namespace OrchardCore.ElsaWorkflows.Controllers.WorkflowDefinitions.Edit;

public class EditViewModel
{
    [Required]
    public string Name { get; set; }
}
