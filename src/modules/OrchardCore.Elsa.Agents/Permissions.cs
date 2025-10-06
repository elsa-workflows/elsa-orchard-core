using OrchardCore.Security.Permissions;

namespace OrchardCore.Elsa.Agents;

public class Permissions : IPermissionProvider
{
    public static readonly Permission ManageAgents = new("ManageElsaAgents", "Manage Elsa agents", isSecurityCritical: true);

    public Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return Task.FromResult<IEnumerable<Permission>>([ManageAgents]);
    }

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
    {
        return new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = [ManageAgents]
            }
        };
    }
}
