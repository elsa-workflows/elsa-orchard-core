using System;
using System.Diagnostics.CodeAnalysis;
using Elsa.Workflows.Core.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Elsa.OrchardCore.Extensions;

public static class ActivityExtensions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static LocalizedHtmlString GetLocalizedStatus(this IHtmlLocalizer H, WorkflowSubStatus status)
    {
        return status switch
        {
            WorkflowSubStatus.Cancelled => H["Cancelled"],
            WorkflowSubStatus.Executing => H["Running"],
            WorkflowSubStatus.Faulted => H["Faulted"],
            WorkflowSubStatus.Finished => H["Finished"],
            WorkflowSubStatus.Suspended => H["Suspended"],
            _ => throw new NotSupportedException(),
        };
    }
}