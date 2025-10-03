using System.ComponentModel.DataAnnotations;

namespace OrchardCore.Elsa.Controllers.WorkflowDefinitions.Create;

public class CreateViewModel
{
    [Required] public string Name { get; set; } = null!;
}
