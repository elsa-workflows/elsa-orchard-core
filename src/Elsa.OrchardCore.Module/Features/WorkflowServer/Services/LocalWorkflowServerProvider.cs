using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Services;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Features.WorkflowServer.Services
{
    using WorkflowServer = Elsa.OrchardCore.Models.WorkflowServer;

    public class LocalWorkflowServerProvider : IWorkflowServerProvider
    {
        private readonly ISiteService _siteService;

        public LocalWorkflowServerProvider(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public async Task<IEnumerable<WorkflowServer>> ListWorkflowServersAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _siteService.GetSiteSettingsAsync();

            var server = new WorkflowServer
            {
                WorkflowServerId = "",
                Name = settings.SiteName,
                Url = new Uri($"{settings.BaseUrl}/graphql")
            };

            return new[] { server };
        }
    }
}