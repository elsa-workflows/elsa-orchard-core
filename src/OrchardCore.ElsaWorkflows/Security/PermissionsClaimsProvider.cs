using System.Security.Claims;
using System.Threading.Tasks;
using OrchardCore.Users;
using OrchardCore.Users.Services;

namespace OrchardCore.ElsaWorkflows.Security;

public class PermissionsClaimsProvider : IUserClaimsProvider
{
    public Task GenerateAsync(IUser user, ClaimsIdentity claims)
    {
        claims.AddClaim(new Claim("permissions", "*"));
        return Task.CompletedTask;
    }
}
