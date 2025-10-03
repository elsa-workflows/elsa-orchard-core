using System.Reflection;
using Elsa.Workflows.UIHints.CheckList;
using Open.Linq.AsyncExtensions;
using OrchardCore.ContentManagement.Metadata;

namespace OrchardCore.Elsa.Contents.UIHints;

public class ContentTypeCheckListOptionsProvider(IContentDefinitionManager contentDefinitionManager) : CheckListOptionsProviderBase
{
    protected override async ValueTask<ICollection<CheckListItem>> GetItemsAsync(PropertyInfo propertyInfo, object? context, CancellationToken cancellationToken)
    {
        var creatableContentTypeDefinitions = await contentDefinitionManager.ListTypeDefinitionsAsync().ToList();
        return creatableContentTypeDefinitions.Select(x => new CheckListItem(x.DisplayName, x.Name)).ToList();
    }
}