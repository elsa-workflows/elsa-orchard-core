using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class GetWorkflowDefinitions
        : IGetWorkflowDefinitions
    {
        public GetWorkflowDefinitions(
            global::System.Collections.Generic.IReadOnlyList<global::Elsa.OrchardCore.IWorkflowDefinitionVersion> workflowDefinitions)
        {
            WorkflowDefinitions = workflowDefinitions;
        }

        public global::System.Collections.Generic.IReadOnlyList<global::Elsa.OrchardCore.IWorkflowDefinitionVersion> WorkflowDefinitions { get; }
    }
}
