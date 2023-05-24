import {Component, Prop, h} from '@stencil/core';
import {Container, ServerSettings, StudioService, WorkflowDefinitionEditorService, WorkflowDefinitionManager} from "@elsa-workflows/elsa-workflows-designer";

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

        // const instanceId = this.instanceId;
        // const definitionManager = Container.get(WorkflowDefinitionManager);
        // const definition = await definitionManager.getWorkflow(definitionId, {isLatest: true});
        // const definitionEditorService = Container.get(WorkflowDefinitionEditorService);
        // definitionEditorService.show(definition);
    }
}
