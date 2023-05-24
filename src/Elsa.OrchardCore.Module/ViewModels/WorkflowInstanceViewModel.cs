using System;
using Elsa.Workflows.Management.Entities;

namespace Elsa.OrchardCore.ViewModels;

public record WorkflowInstanceViewModel(WorkflowDefinition WorkflowDefinition, WorkflowInstance WorkflowInstance, Uri ServerUrl, string InstanceId);