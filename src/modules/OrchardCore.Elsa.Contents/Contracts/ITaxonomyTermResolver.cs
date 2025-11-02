using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Contents.Contracts;

public interface ITaxonomyTermResolver
{
    Task<ContentItem> ResolveTermAsync(string taxonomyHandle, string alias, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentItem>> ResolveTermsAsync(string taxonomyHandle, IEnumerable<string> aliases, CancellationToken cancellationToken = default);
}