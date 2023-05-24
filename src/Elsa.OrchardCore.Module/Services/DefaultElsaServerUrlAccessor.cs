using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.OrchardCore.Contracts;
using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;

namespace Elsa.OrchardCore.Services;

public class DefaultElsaServerUrlAccessor : IElsaServerUrlAccessor
{
    private readonly ISiteService _siteService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ShellSettings _shellSettings;

    public DefaultElsaServerUrlAccessor(ISiteService siteService, IHttpContextAccessor httpContextAccessor, ShellSettings shellSettings)
    {
        _siteService = siteService;
        _httpContextAccessor = httpContextAccessor;
        _shellSettings = shellSettings;
    }
    
    public async Task<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default)
    {
        var siteSettings = await _siteService.GetSiteSettingsAsync();
        var baseUrl = !string.IsNullOrEmpty(siteSettings.BaseUrl) ? siteSettings.BaseUrl : GetBaseUrl();
        var serverUrl = new Uri($"{baseUrl}/elsa/api");
        
        return serverUrl;
    }

    public string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        if (request == null)
            throw new InvalidOperationException("Cannot determine base URL because the current HTTP context is null.");
        
        var shellPath = _shellSettings.RequestUrlPrefix;
        return $"{request.Scheme}://{request.Host}{shellPath}";
    }

}