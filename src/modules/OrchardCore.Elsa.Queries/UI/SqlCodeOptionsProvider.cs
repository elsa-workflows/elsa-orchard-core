using System.Reflection;
using Elsa.Workflows.UIHints.CodeEditor;

namespace OrchardCore.Elsa.Queries.UI;

public class SqlCodeOptionsProvider : CodeEditorOptionsProviderBase
{
    protected override string GetLanguage(PropertyInfo propertyInfo, object? context) => "sql";
}