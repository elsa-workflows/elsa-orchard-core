import {Component, Prop, h} from '@stencil/core';
import {Container, StudioService, WorkflowDefinitionEditorService, WorkflowDefinitionManager} from "@elsa-workflows/elsa-workflows-designer";

@Component({
    tag: 'oc-elsa-definition-editor',
    styleUrl: 'definition-editor.css',
    shadow: false,
})
export class DefinitionEditor {
    @Prop({attribute: 'server-url'}) serverUrl: string;
    @Prop({attribute: 'definition-id'}) definitionId?: string;

    render() {
        const serverUrl = this.serverUrl;
        return <elsa-studio serverUrl={serverUrl} disableAuth={true}/>;
    }

    async componentDidLoad() {
        const definitionId = this.definitionId;
        const definitionManager = Container.get(WorkflowDefinitionManager);
        const definition = await definitionManager.getWorkflow(definitionId, {isLatest: true});
        const definitionEditorService = Container.get(WorkflowDefinitionEditorService);
        definitionEditorService.show(definition);
    }
}
