using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class WorkflowDefinitionVersion
        : IWorkflowDefinitionVersion
    {
        public WorkflowDefinitionVersion(
            string id, 
            string definitionId, 
            string name, 
            string description, 
            int version, 
            bool isLatest, 
            bool isPublished, 
            bool isSingleton, 
            bool isDisabled)
        {
            Id = id;
            DefinitionId = definitionId;
            Name = name;
            Description = description;
            Version = version;
            IsLatest = isLatest;
            IsPublished = isPublished;
            IsSingleton = isSingleton;
            IsDisabled = isDisabled;
        }

        public string Id { get; }

        public string DefinitionId { get; }

        public string Name { get; }

        public string Description { get; }

        public int Version { get; }

        public bool IsLatest { get; }

        public bool IsPublished { get; }

        public bool IsSingleton { get; }

        public bool IsDisabled { get; }
    }
}
