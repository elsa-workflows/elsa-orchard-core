import {Component, Prop, h} from '@stencil/core';
import {Container, ServerSettings, WorkflowDefinitionManager, WorkflowInstancesApi, WorkflowInstanceViewerService} from "@elsa-workflows/elsa-workflows-designer";


@Component({
    tag: 'oc-elsa-instance-viewer',
    shadow: false,
})
export class InstanceViewer {
    @Prop({attribute: 'server-url'}) serverUrl: string;
    @Prop({attribute: 'instance-id'}) instanceId?: string;

    render() {
        const serverUrl = this.serverUrl;
        return <elsa-studio serverUrl={serverUrl} disableAuth={true}/>;
    }
    
    async componentWillLoad() {
        const settings = Container.get(ServerSettings);
        settings.baseAddress = this.serverUrl;

        const instanceId = this.instanceId;
        const workflowInstancesApi = Container.get(WorkflowInstancesApi);
        const instance = await workflowInstancesApi.get({id: instanceId});
        const definitionId = instance.definitionId;
        const definitionManager = Container.get(WorkflowDefinitionManager);
        const definition = await definitionManager.getWorkflow(definitionId, {isLatest: true});
        const instanceViewerService = Container.get(WorkflowInstanceViewerService);
        instanceViewerService.show(definition, instance);
    }
}
