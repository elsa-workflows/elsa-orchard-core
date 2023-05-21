using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Features.LocalWorkflowServer.Services;

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
        return new LocalWorkflowServerClient(new LocalWorkflowServerDefinitionsApiClient());
    }

    public async ValueTask<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _siteService.GetSiteSettingsAsync();
        var baseUrl = settings.BaseUrl ?? throw new Exception("Base URL is not set");
        var elsaApiUrl = $"{baseUrl.TrimEnd('/')}/elsa/api";
        return new Uri(elsaApiUrl);
    }
}