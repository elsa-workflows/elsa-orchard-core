using System.Threading.Tasks;
using Elsa.Email.Activities;
using Elsa.Extensions;
using Elsa.Workflows.Core.Contracts;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Emails.StartupTasks;

public class RegisterActivitiesStartupTask : ModularTenantEvents
{
    private readonly IActivityRegistry _activityRegistry;

    public RegisterActivitiesStartupTask(IActivityRegistry activityRegistry)
    {
        _activityRegistry = activityRegistry;
    }
    
    public override async Task ActivatingAsync()
    {
        await _activityRegistry.RegisterAsync<SendEmail>();
    }
}