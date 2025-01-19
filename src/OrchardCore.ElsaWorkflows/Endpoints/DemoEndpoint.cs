using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.ElsaWorkflows.Endpoints;

public class DemoEndpoint : FastEndpoints.EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/demo");
        //AllowAnonymous();
        Permissions("demo", "*");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var claims = User.Claims;
        await SendOkAsync(new { Message = "Demo" }, ct);
    }
}