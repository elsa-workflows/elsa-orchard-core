using System.Threading.Tasks;
using Elsa.Expressions.Contracts;
using Elsa.Workflows.Management.Contracts;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Core.StartupTasks;

public class RegisterDescriptorsStartupTask : ModularTenantEvents
{
    private readonly IActivityRegistryPopulator _activityRegistryPopulator;
    private readonly IExpressionSyntaxRegistryPopulator _expressionSyntaxRegistryPopulator;

    public RegisterDescriptorsStartupTask(IActivityRegistryPopulator activityRegistryPopulator, IExpressionSyntaxRegistryPopulator expressionSyntaxRegistryPopulator)
    {
        _activityRegistryPopulator = activityRegistryPopulator;
        _expressionSyntaxRegistryPopulator = expressionSyntaxRegistryPopulator;
    }
    
    public override async Task ActivatingAsync()
    {
        await _activityRegistryPopulator.PopulateRegistryAsync();
        await _expressionSyntaxRegistryPopulator.PopulateRegistryAsync();
    }
}