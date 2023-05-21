using System.Threading.Tasks;
using Elsa.Extensions;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Core.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement.Notify;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Activities.UI;

[Activity("Elsa", "UI", "Display a notification.")]
[PublicAPI]
public class Notify : CodeActivity
{
    [Input(Description = "The notification type.")]
    public Input<NotifyType> NotificationType { get; set; } = new(NotifyType.Information);
    
    [Input(Description = "The notification message.")]
    public Input<string> Message { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var notifier = context.GetRequiredService<INotifier>();
        var notificationType = NotificationType.Get(context);
        var message = Message.Get(context);
        var localizedMessage = new LocalizedHtmlString(nameof(Notify), message);
        await notifier.AddAsync(notificationType, localizedMessage);
    }
}