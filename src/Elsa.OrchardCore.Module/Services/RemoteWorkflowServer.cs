using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Client;
using Elsa.Client.Extensions;
using Elsa.OrchardCore.Features.RemoteWorkflows.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.OrchardCore.Services
{
    public class RemoteWorkflowServer : IWorkflowServer
    {
        public RemoteWorkflowServer(string id, string name, Uri serverUrl)
        {
            Id = id;
            Name = name;
            ServerUrl = serverUrl;
        }
        
        public string Id { get;  }
        public string Name { get; }
        public Uri ServerUrl { get; }

        public IWorkflowServerClient CreateClient()
        {
            var services = new ServiceCollection()
                .AddElsaClient(options => options.ServerUrl = ServerUrl)
                .BuildServiceProvider();

            var elsaClient = services.GetRequiredService<IElsaClient>();
            return new RemoteWorkflowServerClient(elsaClient);
        }

        public ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default) => new(ServerUrl);
    }
}