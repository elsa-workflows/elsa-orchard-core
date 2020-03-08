using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial interface IWorkflowDefinitionVersion
    {
        string Id { get; }

        string DefinitionId { get; }

        string Name { get; }

        string Description { get; }

        int Version { get; }

        bool IsLatest { get; }

        bool IsPublished { get; }

        bool IsSingleton { get; }

        bool IsDisabled { get; }
    }
}
