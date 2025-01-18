using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.ElsaWorkflows.Indexes;
using OrchardCore.Navigation;
using YesSql;

namespace OrchardCore.ElsaWorkflows.Endpoints.WorkflowDefinitions.List;

[Admin("ElsaWorkflows/WorkflowDefinitions/List")]
public class WorkflowDefinitionsController(
    IAuthorizationService authorizationService,
    ISession session,
    IShapeFactory shapeFactory,
    IContentItemDisplayManager contentItemDisplayManager,
    IStringLocalizer<WorkflowDefinitionsController> stringLocalizer,
    IOptions<PagerOptions> pagerOptions) : Controller, IUpdateModel
{
    public async Task<IActionResult> List(ListOptions? options, PagerParameters pagerParameters)
    {
        if (!await authorizationService.AuthorizeAsync(User, Permissions.ManageWorkflows))
            return Forbid();
        
        var pager = new Pager(pagerParameters, pagerOptions.Value.GetPageSize());
        options ??= new();

        var query = session.Query<ContentItem, ContentItemIndex>(x => x.Latest).With<WorkflowDefinitionIndex>();

        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x => x.Name.Contains(options.Search));
        }

        switch (options.Order)
        {
            case Order.Name:
                query = query.OrderBy(u => u.Name);
                break;
        }

        var count = await query.CountAsync();

        var contentItems = await query
            .Skip(pager.GetStartIndex())
            .Take(pager.PageSize)
            .ListAsync();

        // Maintain previous route data when generating page links.
        var routeData = new RouteData();
        routeData.Values.Add("Options.Filter", options.Filter);
        routeData.Values.Add("Options.Order", options.Order);
        
        if (!string.IsNullOrEmpty(options.Search)) 
            routeData.Values.TryAdd("Options.Search", options.Search);
        
        var contentItemSummaries = new System.Collections.Generic.List<dynamic>();
        foreach (var contentItem in contentItems) 
            contentItemSummaries.Add(await contentItemDisplayManager.BuildDisplayAsync(contentItem, this, "SummaryAdmin"));

        var pagerShape = await shapeFactory.PagerAsync(pager, count, routeData);
        var model = new ListViewModel
        {
            Items = contentItemSummaries
                .Select(x => new ListItemViewModel
                {
                    //WorkflowDefinition = x,
                    ContentItemSummary = x,
                    Id = x.ContentItem.Id,
                    Name = x.ContentItem.DisplayText,
                })
                .ToList(),
            Options = options,
            Pager = pagerShape,
        };

        model.Options.WorkflowTypesBulkAction =
        [
            new(stringLocalizer["Delete"], nameof(BulkAction.Delete)),
        ];

        return View(model);
    }
}