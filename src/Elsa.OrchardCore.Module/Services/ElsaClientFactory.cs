using System;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.OrchardCore.Services
{
    public class ElsaClientFactory : IElsaClientFactory
    {
        public IElsaClient GetOrCreateClient(Uri url)
        {
            var services = new ServiceCollection();
            services.AddElsaClient();
            services.AddHttpClient("ElsaClient", httpClient => httpClient.BaseAddress = url);
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IElsaClient>();
        }
    }
}