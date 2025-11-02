using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Elsa.Contents.Contracts;
using OrchardCore.Modules;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Title.Models;

namespace OrchardCore.Elsa.Contents.Services;

public class TaxonomyTermResolver(
    IContentDefinitionManager contentDefinitionManager,
    IContentManager contentManager,
    IContentHandleManager contentHandleManager,
    IEnumerable<IContentHandler> contentHandlers,
    ILogger<TaxonomyTermResolver> logger) : ITaxonomyTermResolver
{
    public async Task<ContentItem> ResolveTermAsync(string taxonomyHandle, string alias, CancellationToken cancellationToken = default)
    {
        var terms = await ResolveTermsAsync(taxonomyHandle, [alias], cancellationToken);
        return terms.First();
    }

    public async Task<IEnumerable<ContentItem>> ResolveTermsAsync(string taxonomyHandle, IEnumerable<string> aliases, CancellationToken cancellationToken = default)
    {
        var contentTypeDefinition = await contentDefinitionManager.GetTypeDefinitionAsync("Taxonomy");
        var versionOptions = contentTypeDefinition.IsDraftable() ? VersionOptions.DraftRequired : VersionOptions.Latest;
        var tagsTaxonomyContentItemId = await contentHandleManager.GetContentItemIdAsync(taxonomyHandle);
        var tagsTaxonomyContentItem = await contentManager.GetAsync(tagsTaxonomyContentItemId, versionOptions);

        if (tagsTaxonomyContentItem == null)
            throw new InvalidOperationException($"Could not find taxonomy with handle {taxonomyHandle}");

        return await ResolveTermAsync(tagsTaxonomyContentItem, contentTypeDefinition, aliases, cancellationToken);
    }

    public async Task<IEnumerable<ContentItem>> ResolveTermAsync(ContentItem tagsTaxonomyContentItem, IEnumerable<string> requestedTags, CancellationToken cancellationToken = default)
    {
        var contentTypeDefinition = await contentDefinitionManager.GetTypeDefinitionAsync("Taxonomy");
        return await ResolveTermAsync(tagsTaxonomyContentItem, contentTypeDefinition, requestedTags, cancellationToken);
    }

    private async Task<IEnumerable<ContentItem>> ResolveTermAsync(ContentItem tagsTaxonomyContentItem, ContentTypeDefinition contentTypeDefinition, IEnumerable<string> requestedTags, CancellationToken cancellationToken = default)
    {
        var taxonomyPart = tagsTaxonomyContentItem.ContentItem.As<TaxonomyPart>();
        var existingTagTerms = taxonomyPart.Terms.Where(t => t.Has<TitlePart>() && requestedTags.Contains(t.As<TitlePart>().Title, StringComparer.OrdinalIgnoreCase)).ToList();
        var allTagTerms = existingTagTerms.ToList();
        var existingTags = existingTagTerms.Select(t => t.As<TitlePart>().Title).ToList();
        var missingTagTerms = requestedTags.Except(existingTags).ToList();

        foreach (var missingTagTerm in missingTagTerms)
        {
            var newTagTerm = await CreateTagTermContentItem(tagsTaxonomyContentItem, missingTagTerm);
            allTagTerms.Add(newTagTerm);
        }

        if (!missingTagTerms.Any())
            return allTagTerms;

        if (contentTypeDefinition.IsDraftable())
            await contentManager.PublishAsync(tagsTaxonomyContentItem);
        else
            await contentManager.SaveDraftAsync(tagsTaxonomyContentItem);

        return allTagTerms;
    }

    private async Task<ContentItem> CreateTagTermContentItem(ContentItem tagsTaxonomy, string tagName)
    {
        // Create a tag term but only run content handlers, not content item display manager update editor.
        // This creates empty parts if parts are attached to the tag term, with empty data.
        // But still generates valid auto-route paths from the handler. 
        var tagsTaxonomyPart = tagsTaxonomy.As<TaxonomyPart>();
        var termContentItem = await contentManager.NewAsync(tagsTaxonomyPart.TermContentType);
        termContentItem.DisplayText = tagName;
        termContentItem.Alter<TitlePart>(part => part.Title = tagName);
        termContentItem.Weld<TermPart>();
        termContentItem.Alter<TermPart>(t => { t.TaxonomyContentItemId = tagsTaxonomy.ContentItemId; });
        var updateContentContext = new UpdateContentContext(termContentItem);
        await contentHandlers.InvokeAsync((handler, context) => handler.UpdatingAsync(context), updateContentContext, logger);
        await contentHandlers.Reverse().InvokeAsync((handler, context) => handler.UpdatedAsync(context), updateContentContext, logger);
        tagsTaxonomy.Alter<TaxonomyPart>(part => part.Terms.Add(termContentItem));

        return termContentItem;
    }
}