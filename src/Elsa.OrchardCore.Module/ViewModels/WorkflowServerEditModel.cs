using System;
using System.ComponentModel.DataAnnotations;

namespace Elsa.OrchardCore.ViewModels;

public class WorkflowServerEditModel
{
    [Required] public string Name { get; set; } = default!;
    [Required] public Uri Url { get; set; } = default!;
}