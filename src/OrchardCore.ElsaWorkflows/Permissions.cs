using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace OrchardCore.ElsaWorkflows;

public class Permissions : IPermissionProvider
{
    public static readonly Permission ManageWorkflows = new("ManageWorkflows", "Manage workflows", isSecurityCritical: true);

    public Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return Task.FromResult(new[] { ManageWorkflows }.AsEnumerable());
    }

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
    {
        return
        [
            new()
            {
                Name = "Administrator",
                Permissions = [ManageWorkflows]
            }
        ];
    }
}