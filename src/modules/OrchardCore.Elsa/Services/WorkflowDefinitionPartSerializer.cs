using Elsa.Workflows;
using Elsa.Workflows.Management.Models;
using OrchardCore.Elsa.Parts;

namespace OrchardCore.Elsa.Services;

public class WorkflowDefinitionPartSerializer(IApiSerializer apiSerializer, WorkflowDefinitionPartMapper mapper)
{
    public WorkflowDefinitionModel UpdateSerializedData(WorkflowDefinitionPart part)
    {
        var model = mapper.MapModel(part);
        part.SerializedData = apiSerializer.Serialize(model);
        return model;
    }
}