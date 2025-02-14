using Elsa.Workflows;
using Elsa.Workflows.Management.Models;
using OrchardCore.ElsaWorkflows.Parts;

namespace OrchardCore.ElsaWorkflows.Services;

public class WorkflowDefinitionPartSerializer(IApiSerializer apiSerializer, WorkflowDefinitionPartMapper mapper)
{
    public WorkflowDefinitionModel UpdateSerializedData(WorkflowDefinitionPart part)
    {
        var model = mapper.MapModel(part);
        part.SerializedData = apiSerializer.Serialize(model);
        return model;
    }
}