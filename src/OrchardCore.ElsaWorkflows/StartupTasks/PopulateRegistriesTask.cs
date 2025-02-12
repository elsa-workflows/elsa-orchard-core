using System.Threading.Tasks;
using Elsa.Workflows.Runtime;
using OrchardCore.Modules;

namespace OrchardCore.ElsaWorkflows.StartupTasks;

public class PopulateRegistriesTask(IRegistriesPopulator registriesPopulator) : ModularTenantEvents
{
    public override async Task ActivatedAsync()
    {
        await registriesPopulator.PopulateAsync();
    }
}