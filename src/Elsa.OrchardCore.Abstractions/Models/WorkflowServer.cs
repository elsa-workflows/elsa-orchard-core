using System;

namespace Elsa.OrchardCore.Models
{
    public class WorkflowServer
    {
        public int Id { get; set; }
        public string WorkflowServerId { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
    }
}