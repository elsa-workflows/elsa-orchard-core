using System;

namespace Elsa.OrchardCore.Models
{
    public class RemoteWorkflowServerRecord
    {
        public int Id { get; set; }
        public string WorkflowServerId { get; set; } = default!;
        public string Name { get; set; }= default!;
        public Uri Url { get; set; }= default!;
    }
}