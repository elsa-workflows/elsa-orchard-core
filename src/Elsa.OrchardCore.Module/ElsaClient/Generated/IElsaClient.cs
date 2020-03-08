using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial interface IElsaClient
    {
        Task<IOperationResult<global::Elsa.OrchardCore.IGetWorkflowDefinitions>> GetWorkflowDefinitionsAsync(
            Optional<global::Elsa.OrchardCore.VersionOptionsInput> version = default,
            CancellationToken cancellationToken = default);

        Task<IOperationResult<global::Elsa.OrchardCore.IGetWorkflowDefinitions>> GetWorkflowDefinitionsAsync(
            GetWorkflowDefinitionsOperation operation,
            CancellationToken cancellationToken = default);
    }
}
