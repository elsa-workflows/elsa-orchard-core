using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using JetBrains.Annotations;
using OrchardCore.DisplayManagement.Notify;

namespace OrchardCore.Elsa.UI.Activities;

[Activity("OrchardCore.UI", "UI", "Displays a notification.")]
[UsedImplicitly]
public class DisplayNotification : CodeActivity
{
    [Input(Description = "The notification type to display.")]
    public Input<NotifyType> NotificationType { get; set; } = null!;
    
    [Input(Description = "The message to display.")]
    public Input<string> Message { get; set; } = null!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var message = Message.Get(context);
        var notificationType = NotificationType.Get(context);
        var notifier = context.GetRequiredService<INotifier>();

        // The notification message can contain HTML by design
        await notifier.AddAsync(notificationType, new(nameof(DisplayNotification), message));
    }
}