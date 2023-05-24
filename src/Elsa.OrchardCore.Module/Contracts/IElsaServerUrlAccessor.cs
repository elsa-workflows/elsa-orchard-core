using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa.OrchardCore.Contracts;

/// <summary>
/// Provides the public URL of the Elsa server.
/// </summary>
public interface IElsaServerUrlAccessor
{
    Task<Uri> GetServerUrlAsync(CancellationToken cancellationToken = default);
}