using Elsa.Common;
using Elsa.Mediator.Contracts;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Materializers;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Management.Notifications;
using Elsa.Workflows.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Parts;

namespace OrchardCore.Elsa.Services;

public class ContentItemWorkflowDefinitionPublisher(
    IContentManager contentManager,
    ISystemClock systemClock,
    IActivitySerializer activitySerializer,
    IWorkflowDefinitionStore workflowDefinitionStore,
    IWorkflowDefinitionService workflowDefinitionService,
    IWorkflowValidator workflowValidator,
    IMediator mediator,
    WorkflowDefinitionPartMapper workflowDefinitionPartMapper,
    WorkflowDefinitionPartSerializer workflowDefinitionPartSerializer) : IWorkflowDefinitionPublisher
{
    public WorkflowDefinition New(IActivity? root = null)
    {
        throw new("Not implemented. Use NewAsync instead.");
    }

    public async Task<WorkflowDefinition> NewAsync(IActivity? root = null, CancellationToken cancellationToken = default)
    {
        const int version = 1;
        root ??= new Sequence();

        var contentItem = await contentManager.NewAsync("WorkflowDefinition");
        var definitionId = contentItem.ContentItemId;

        return new()
        {
            Id = string.Empty,
            DefinitionId = definitionId,
            Version = version,
            IsLatest = true,
            IsPublished = false,
            CreatedAt = systemClock.UtcNow,
            StringData = activitySerializer.Serialize(root),
            MaterializerName = JsonWorkflowMaterializer.MaterializerName
        };
    }

    public async Task<PublishWorkflowDefinitionResult> PublishAsync(string definitionId, CancellationToken cancellationToken = default)
    {
        var filter = WorkflowDefinitionHandle.ByDefinitionId(definitionId, global::Elsa.Common.Models.VersionOptions.Latest).ToFilter();
        var definition = await workflowDefinitionStore.FindAsync(filter, cancellationToken);

        if (definition == null)
            return new(false, new List<WorkflowValidationError>
            {
                new("Workflow definition not found.")
            }, new([]));

        return await PublishAsync(definition, cancellationToken);
    }

    public async Task<PublishWorkflowDefinitionResult> PublishAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        var workflowGraph = await workflowDefinitionService.MaterializeWorkflowAsync(definition, cancellationToken);
        var validationErrors = (await workflowValidator.ValidateAsync(workflowGraph.Workflow, cancellationToken)).ToList();

        if (validationErrors.Any())
            return new(false, validationErrors, new([]));

        var contentItem = await contentManager.GetAsync(definition.DefinitionId, VersionOptions.DraftRequired);

        if (definition.IsPublished)
            definition.Version++;

        definition.IsLatest = true;
        definition.IsPublished = true;
        definition.Id = contentItem.ContentItemVersionId;

        contentItem.Alter<WorkflowDefinitionPart>(part =>
        {
            part.DefinitionId = definition.DefinitionId;
            part.DefinitionVersionId = definition.Id;
            part.IsPublished = definition.IsPublished;
            part.IsLatest = definition.IsLatest;
            part.Name = definition.Name;
            part.Description = definition.Description;
            part.IsReadonly = definition.IsReadonly;
            part.IsSystem = definition.IsSystem;
            part.MaterializerName = definition.MaterializerName;
            part.ProviderName = definition.ProviderName;
            part.ToolVersion = definition.ToolVersion;
            part.UsableAsActivity = definition.Options.UsableAsActivity == true;
            workflowDefinitionPartSerializer.UpdateSerializedData(part);
        });

        await contentManager.PublishAsync(contentItem);
        var affectedWorkflows = new AffectedWorkflows(new List<WorkflowDefinition>());
        await mediator.SendAsync(new WorkflowDefinitionPublished(definition, affectedWorkflows), cancellationToken);
        return new(true, validationErrors, affectedWorkflows);
    }

    public async Task<WorkflowDefinition?> RetractAsync(string definitionId, CancellationToken cancellationToken = default)
    {
        var contentItem = await contentManager.GetAsync(definitionId, VersionOptions.Latest);

        if (contentItem == null)
            return null;

        await contentManager.UnpublishAsync(contentItem);
        return workflowDefinitionPartMapper.Map(contentItem.As<WorkflowDefinitionPart>());
    }

    public async Task<WorkflowDefinition> RetractAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        var contentItem = await contentManager.GetAsync(definition.DefinitionId, VersionOptions.Latest);
        await contentManager.UnpublishAsync(contentItem);
        return workflowDefinitionPartMapper.Map(contentItem.As<WorkflowDefinitionPart>());
    }

    public async Task<WorkflowDefinition?> GetDraftAsync(string definitionId, global::Elsa.Common.Models.VersionOptions versionOptions, CancellationToken cancellationToken = default)
    {
        var contentItem = await contentManager.GetAsync(definitionId, VersionOptions.DraftRequired);

        if (contentItem == null)
            return null;

        contentItem.Alter<WorkflowDefinitionPart>(part =>
        {
            var isNewVersion = part.DefinitionVersionId != contentItem.ContentItemVersionId;
            part.IsPublished = false;
            part.IsLatest = true;
            part.DefinitionVersionId = contentItem.ContentItemVersionId;

            if (isNewVersion)
                part.Version++;

            workflowDefinitionPartSerializer.UpdateSerializedData(part);
        });

        return workflowDefinitionPartMapper.Map(contentItem.As<WorkflowDefinitionPart>());
    }

    public async Task<WorkflowDefinition> SaveDraftAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        await workflowDefinitionStore.SaveAsync(definition, cancellationToken);
        return definition;
    }
}