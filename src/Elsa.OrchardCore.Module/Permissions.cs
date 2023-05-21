using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace Elsa.OrchardCore;

public class Permissions : IPermissionProvider
{
    public static readonly Permission ManageWorkflowServers = new("ManageWorkflowServers", "Manage workflow servers", isSecurityCritical: true);
    public static readonly Permission ManageWorkflows = new("ManageWorkflows", "Manage workflows", isSecurityCritical: true);

    public Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return Task.FromResult(new[] { ManageWorkflowServers, ManageWorkflows }.AsEnumerable());
    }

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
    {
        return new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[] { ManageWorkflowServers, ManageWorkflows }
            }
        };
    }
}