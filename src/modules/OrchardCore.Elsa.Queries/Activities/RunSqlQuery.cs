using Dapper;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using Fluid;
using Fluid.Values;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using OrchardCore.Elsa.Queries.UI;
using OrchardCore.Liquid;
using OrchardCore.Queries.Sql;
using YesSql;

namespace OrchardCore.Elsa.Queries.Activities;

[Activity("OrchardCore.Queries", "Queries", "Executes a SQL query and returns the results.", DisplayName = "Run SQL Query")]
[UsedImplicitly]
public class RunSqlQuery : CodeActivity<ICollection<dynamic>>
{
    [Input(
        Description = "The SQL query to execute.",
        UIHint = InputUIHints.CodeEditor,
        UIHandler = typeof(SqlCodeOptionsProvider)
    )]
    public Input<string> Query { get; set; } = null!;

    [Input(
        Description = "An optional dictionary of parameters to pass to the query.",
        UIHint = InputUIHints.Dictionary
    )]
    public Input<IDictionary<string, object>> Parameters { get; set; } = null!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var store = context.GetRequiredService<IStore>();
        var liquidTemplateManager = context.GetRequiredService<ILiquidTemplateManager>();
        var templateOptions = context.GetRequiredService<IOptions<TemplateOptions>>().Value;
        var query = Query.Get(context);
        var parameters = Parameters.GetOrDefault(context) ?? new Dictionary<string, object>();
        var dialect = store.Configuration.SqlDialect;
        var tokenizedQuery = await liquidTemplateManager.RenderStringAsync(query, NullEncoder.Default, parameters.Select(x => new KeyValuePair<string, FluidValue>(x.Key, FluidValue.Create(x.Value, templateOptions))));

        if (SqlParser.TryParse(tokenizedQuery, store.Configuration.Schema, dialect, store.Configuration.TablePrefix, parameters, out var rawQuery, out var messages))
        {
            await using var connection = store.Configuration.ConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            var documents = await connection.QueryAsync(rawQuery, parameters);

            context.SetResult(documents);
        }
        else
        {
            throw new SqlParserException("Failed to parse SQL query.");
        }
    }
}