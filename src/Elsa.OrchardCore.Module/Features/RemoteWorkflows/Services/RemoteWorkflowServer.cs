using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Api.Client.Contracts;
using Elsa.Api.Client.Extensions;
using Elsa.OrchardCore.Contracts;
using Elsa.OrchardCore.Features.LocalWorkflowServer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.OrchardCore.Features.RemoteWorkflows.Services;

public class RemoteWorkflowServer : IWorkflowServer
{
    public RemoteWorkflowServer(string id, string name, Uri serverUrl, string apiKey)
    {
        Id = id;
        Name = name;
        ServerUrl = serverUrl;
        ApiKey = apiKey;
    }
        
    public string Id { get;  }
    public string Name { get; }
    public Uri ServerUrl { get; }
    public string ApiKey { get; set; }

    public IWorkflowServerClient CreateClient()
    {
        var services = new ServiceCollection()
            .AddElsaClient(options =>
            {
                options.BaseAddress = ServerUrl;
                options.ApiKey = ApiKey;
            })
            .BuildServiceProvider();

        var elsaClient = services.GetRequiredService<IElsaClient>();
        return new RemoteWorkflowServerClient(new LocalWorkflowServerDefinitionsApiClient());
    }

    public ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default) => new(ServerUrl);
}