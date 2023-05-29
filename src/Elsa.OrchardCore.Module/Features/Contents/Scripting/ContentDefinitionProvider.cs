using System.Collections.Generic;
using Elsa.JavaScript.TypeDefinitions.Abstractions;
using Elsa.JavaScript.TypeDefinitions.Contracts;
using Elsa.JavaScript.TypeDefinitions.Models;
using OrchardCore.ContentManagement;

namespace Elsa.OrchardCore.Features.Contents.Scripting;

/// <summary>
/// Produces <see cref="TypeDefinition"/>s for content-related types.
/// </summary>
internal class ContentDefinitionProvider : TypeDefinitionProvider
{
    private readonly ITypeDescriber _typeDescriber;

    public ContentDefinitionProvider(ITypeDescriber typeDescriber)
    {
        _typeDescriber = typeDescriber;
    }

    protected override IEnumerable<TypeDefinition> GetTypeDefinitions(TypeDefinitionContext context)
    {
        yield return _typeDescriber.DescribeType(typeof(ContentItem));
    }
}