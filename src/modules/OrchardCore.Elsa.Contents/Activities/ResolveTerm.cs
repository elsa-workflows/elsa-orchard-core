using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using JetBrains.Annotations;
using OrchardCore.ContentManagement;
using OrchardCore.Elsa.Contents.Contracts;

namespace OrchardCore.Elsa.Contents.Activities;

[Activity("OrchardCore.Content", "Content", "Resolves a given term alias and returns the content item.")]
[UsedImplicitly]
public class ResolveTerm : CodeActivity<ContentItem>
{
    [Input(Description = "The taxonomy handle to resolve the term alias from.")]
    public Input<string> TaxonomyHandle { get; set; } = null!;
    
    [Input(Description = "The term alias to resolve.")]
    public Input<string> Alias { get; set; } = null!;
    
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var resolver = context.GetRequiredService<ITaxonomyTermResolver>();
        var taxonomyHandle = TaxonomyHandle.Get(context);
        var alias = Alias.Get(context);
        var contentItem = await resolver.ResolveTermAsync(taxonomyHandle, alias, cancellationToken);

        context.SetResult(contentItem);
    }
}