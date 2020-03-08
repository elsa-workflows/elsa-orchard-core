using System;
using System.ComponentModel.DataAnnotations;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowServerEditModel
    {
        [Required] public string Name { get; set; }
        [Required] public Uri Url { get; set; }
    }
}