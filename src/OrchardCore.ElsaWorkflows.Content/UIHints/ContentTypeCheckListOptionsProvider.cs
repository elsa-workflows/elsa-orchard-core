using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Workflows.UIHints.CheckList;
using Open.Linq.AsyncExtensions;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;

namespace OrchardCore.ElsaWorkflows.Content.UIHints;

public class ContentTypeCheckListOptionsProvider(IContentDefinitionManager contentDefinitionManager) : CheckListOptionsProviderBase
{
    protected override async ValueTask<ICollection<CheckListItem>> GetItemsAsync(PropertyInfo propertyInfo, object? context, CancellationToken cancellationToken)
    {
        var creatableContentTypeDefinitions = await contentDefinitionManager.ListTypeDefinitionsAsync().Where(x => x.GetSettings<ContentTypeSettings>().Creatable).ToList();
        return creatableContentTypeDefinitions.Select(x => new CheckListItem(x.DisplayName, x.Name)).ToList();
    }
}