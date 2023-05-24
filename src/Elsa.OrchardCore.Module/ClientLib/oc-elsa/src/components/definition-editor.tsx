import {Component, Prop, h} from '@stencil/core';
import {Container, ServerSettings, StudioInitializingContext, WorkflowDefinitionEditorService, WorkflowDefinitionManager} from "@elsa-workflows/elsa-workflows-designer";

@Component({
    tag: 'oc-elsa-definition-editor',
    shadow: false,
})
export class DefinitionEditor {
    @Prop({attribute: 'server-url'}) serverUrl: string;
    @Prop({attribute: 'definition-id'}) definitionId?: string;

    render() {
        const serverUrl = this.serverUrl;
        return <elsa-studio serverUrl={serverUrl} disableAuth={true} onInitializing={e => this.onInitializing(e)}/>;
    }

    async componentWillLoad() {
        const settings = Container.get(ServerSettings);
        settings.baseAddress = this.serverUrl;

        const definitionId = this.definitionId;
        const definitionManager = Container.get(WorkflowDefinitionManager);
        const definition = await definitionManager.getWorkflow(definitionId, {isLatest: true});
        const definitionEditorService = Container.get(WorkflowDefinitionEditorService);
        definitionEditorService.show(definition);
    }

    private onInitializing(e: CustomEvent<StudioInitializingContext>) {
        e.detail.pluginRegistry.remove('home');
        e.detail.pluginRegistry.remove('login');
    }
}
