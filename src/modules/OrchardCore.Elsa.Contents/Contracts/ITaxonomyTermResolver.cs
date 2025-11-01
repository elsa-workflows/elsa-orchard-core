using OrchardCore.ContentManagement;

namespace OrchardCore.Elsa.Contents.Contracts;

public interface ITaxonomyTermResolver
{
    Task<IEnumerable<ContentItem>> ResolveTermAsync(string taxonomyHandle, IEnumerable<string> requestedTags, CancellationToken cancellationToken = default);
}