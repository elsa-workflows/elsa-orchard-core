using System;

namespace Elsa.OrchardCore.Services
{
    public interface IElsaClientFactory
    {
        IElsaClient GetOrCreateClient(Uri url);
    }
}