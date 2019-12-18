using System.Threading.Tasks;
using Elsa.OrchardCore.Models;
using Elsa.OrchardCore.ViewModels;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Entities;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Drivers
{
    public class ElsaWorkflowsSettingsDisplayDriver : DisplayDriver<ISite>
    {
        public const string GroupId = "elsa";

        public override IDisplayResult Edit(ISite site)
        {
            var settings = site.As<ElsaSettings>();

            return Initialize<ElsaWorkflowsSettingsViewModel>("ElsaWorkflowsSettings_Edit", model =>
                {
                    model.WorkflowServerUrl = settings.WorkflowServerUrl;
                })
                .Location("Content:1")
                .OnGroup(GroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(ISite site, UpdateEditorContext context)
        {
            if (context.GroupId == GroupId)
            {
                var model = new ElsaWorkflowsSettingsViewModel();

                if (await context.Updater.TryUpdateModelAsync(model, Prefix))
                {
                    var settings = site.As<ElsaSettings>();
                    settings.WorkflowServerUrl = model.WorkflowServerUrl?.Trim();
                    site.Put(settings);
                }
            }

            return Edit(site);
        }
    }
}