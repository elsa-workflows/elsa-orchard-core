using System.Collections.Generic;
using Elsa.OrchardCore.Models;

namespace Elsa.OrchardCore.ViewModels
{
    public class WorkflowServersViewModel
    {
        public ICollection<WorkflowServer> WorkflowServers { get; set; } = new List<WorkflowServer>();
    }
}