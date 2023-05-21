using JetBrains.Annotations;

namespace Elsa.OrchardCore.Features.Core.Activities.Contents.ContentItemCreated;

/// <summary>
/// A bookmark payload that contains the content type of the content item that triggered the workflow.
/// </summary>
/// <param name="ContentType">The content type of the content item that triggered the workflow.</param>
[PublicAPI]
public record ContentItemCreatedBookmark(string ContentType);