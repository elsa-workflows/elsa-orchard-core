using Elsa.Studio.Workflows.UI.Contracts;
using Elsa.Studio.Workflows.UI.Models;
using MudBlazor;

namespace OrchardCore.Elsa.Designer;

public class ActivityIconProvider : IActivityDisplaySettingsProvider
{
    public IDictionary<string, ActivityDisplaySettings> GetSettings()
    {
        return new Dictionary<string, ActivityDisplaySettings>
        {
            ["OrchardCore.Content.ContentCreated"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentDraftSaved"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentUpdated"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentPublished"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentUnpublished"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentVersioned"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.ContentDeleted"] = new(OrchardCoreColors.ContentEvent, Icons.Material.Filled.ElectricBolt),
            ["OrchardCore.Content.CreateContent"] = new(OrchardCoreColors.ContentAction, OrchardCoreIcons.Heroicons.DocumentPlus),
            ["OrchardCore.Content.UpdateContent"] = new(OrchardCoreColors.ContentAction, OrchardCoreIcons.Tabler.Pen),
            ["OrchardCore.Content.DeleteContent"] = new(OrchardCoreColors.ContentAction, Icons.Material.Filled.Delete),
            ["OrchardCore.Content.GetContent"] = new(OrchardCoreColors.ContentAction, Icons.Material.Filled.FileOpen),
            ["OrchardCore.Content.PublishContent"] = new(OrchardCoreColors.ContentAction, Icons.Material.Filled.CloudUpload),
            ["OrchardCore.Content.UnpublishContent"] = new(OrchardCoreColors.ContentAction, Icons.Material.Filled.CloudDownload),
            ["OrchardCore.Content.ResolveTerm"] = new(OrchardCoreColors.ContentAction, Icons.Material.Filled.ManageSearch),
            ["OrchardCore.UI.DisplayNotification"] = new(OrchardCoreColors.UIAction, Icons.Material.Outlined.Info),
            ["OrchardCore.Queries.RunSqlQuery"] = new(OrchardCoreColors.Queries, OrchardCoreIcons.Tabler.Database),
        };
    }
}