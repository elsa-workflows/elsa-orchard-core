using System.ComponentModel.DataAnnotations;

namespace OrchardCore.Elsa.Controllers.WorkflowDefinitions.Edit;

public class EditViewModel
{
    [Required] public string DefinitionId { get; set; } = null!;
}