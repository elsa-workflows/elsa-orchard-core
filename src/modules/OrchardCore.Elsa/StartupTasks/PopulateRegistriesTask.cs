using Elsa.Workflows.Runtime;
using OrchardCore.Modules;

namespace OrchardCore.Elsa.StartupTasks;

public class PopulateRegistriesTask(IRegistriesPopulator registriesPopulator) : ModularTenantEvents
{
    public override async Task ActivatedAsync()
    {
        await registriesPopulator.PopulateAsync();
    }
}