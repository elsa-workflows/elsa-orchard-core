using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class GetWorkflowDefinitionsOperation
        : IOperation<IGetWorkflowDefinitions>
    {
        public string Name => "getWorkflowDefinitions";

        public IDocument Document => Queries.Default;

        public OperationKind Kind => OperationKind.Query;

        public Type ResultType => typeof(IGetWorkflowDefinitions);

        public Optional<global::Elsa.OrchardCore.VersionOptionsInput> Version { get; set; }

        public IReadOnlyList<VariableValue> GetVariableValues()
        {
            var variables = new List<VariableValue>();

            if (Version.HasValue)
            {
                variables.Add(new VariableValue("version", "VersionOptionsInput", Version.Value));
            }

            return variables;
        }
    }
}
