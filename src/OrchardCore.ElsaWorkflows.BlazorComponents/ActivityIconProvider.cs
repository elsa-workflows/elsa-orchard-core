using Elsa.Studio.Workflows.UI.Contracts;
using Elsa.Studio.Workflows.UI.Models;
using MudBlazor;

namespace OrchardCore.ElsaWorkflows.BlazorComponents;

public class ActivityIconProvider : IActivityDisplaySettingsProvider
{
    public IDictionary<string, ActivityDisplaySettings> GetSettings()
    {
        return new Dictionary<string, ActivityDisplaySettings>
        {
            ["OrchardCore.Content.ContentCreated"] = new(OrchardCoreColors.ContentCreated, OrchardCoreIcons.Heroicons.DocumentPlus),
            ["OrchardCore.Content.ContentDraftSaved"] = new(OrchardCoreColors.ContentSaved, OrchardCoreIcons.Heroicons.Document),
            ["OrchardCore.Content.ContentUpdated"] = new(OrchardCoreColors.ContentSaved, OrchardCoreIcons.Heroicons.Document),
            ["OrchardCore.Content.ContentPublished"] = new(OrchardCoreColors.ContentPublished, OrchardCoreIcons.Heroicons.Document),
            ["OrchardCore.Content.ContentUnpublished"] = new(OrchardCoreColors.ContentUnpublished, OrchardCoreIcons.Heroicons.Document),
            ["OrchardCore.Content.ContentVersioned"] = new(OrchardCoreColors.ContentCreated, OrchardCoreIcons.Heroicons.DocumentPlus),
            ["OrchardCore.Content.ContentDeleted"] = new(OrchardCoreColors.ContentDeleted, OrchardCoreIcons.Heroicons.DocumentMinus),
            ["OrchardCore.UI.DisplayNotification"] = new(OrchardCoreColors.UI, Icons.Material.Outlined.Info),
        };
    }
}