using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Features.LocalWorkflowServer.Services;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Services
{
    public class LocalWorkflowServer : IWorkflowServer
    {
        private readonly ISiteService _siteService;

        public LocalWorkflowServer(string id, string name, ISiteService siteService)
        {
            _siteService = siteService;
            Id = id;
            Name = name;
        }
        
        public string Id { get;  }
        public string Name { get; }
        
        public IWorkflowServerClient CreateClient()
        {
            return new LocalWorkflowServerClient();
        }

        public async ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _siteService.GetSiteSettingsAsync();
            return new Uri(settings.BaseUrl);
        }
    }
}