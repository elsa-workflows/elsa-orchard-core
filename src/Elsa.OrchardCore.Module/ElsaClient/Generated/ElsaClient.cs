using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class ElsaClient
        : IElsaClient
    {
        private const string _clientName = "ElsaClient";

        private readonly global::StrawberryShake.IOperationExecutor _executor;

        public ElsaClient(global::StrawberryShake.IOperationExecutorPool executorPool)
        {
            _executor = executorPool.CreateExecutor(_clientName);
        }

        public global::System.Threading.Tasks.Task<global::StrawberryShake.IOperationResult<global::Elsa.OrchardCore.IGetWorkflowDefinitions>> GetWorkflowDefinitionsAsync(
            global::StrawberryShake.Optional<global::Elsa.OrchardCore.VersionOptionsInput> version = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {

            return _executor.ExecuteAsync(
                new GetWorkflowDefinitionsOperation { Version = version },
                cancellationToken);
        }

        public global::System.Threading.Tasks.Task<global::StrawberryShake.IOperationResult<global::Elsa.OrchardCore.IGetWorkflowDefinitions>> GetWorkflowDefinitionsAsync(
            GetWorkflowDefinitionsOperation operation,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            return _executor.ExecuteAsync(operation, cancellationToken);
        }
    }
}
