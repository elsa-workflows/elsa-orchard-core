using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Models;
using OrchardCore.ContentManagement.Metadata;

namespace Elsa.OrchardCore.Services;

public class ContentTypeOptionsProvider : IActivityPropertyOptionsProvider
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public ContentTypeOptionsProvider(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    public ValueTask<object> GetOptionsAsync(PropertyInfo property, CancellationToken cancellationToken = default)
    {
        var contentTypes = _contentDefinitionManager.ListTypeDefinitions().ToList();

        var selectListItems = contentTypes
            .Select(x => new SelectListItem(x.DisplayName, x.Name))
            .OrderBy(x => x.Text)
            .ToList();
        
        return new(selectListItems);
    }
}