import {Component, Prop, h} from '@stencil/core';
import {Container, PluginRegistry, ServerSettings, StudioInitializingContext, WorkflowDefinitionEditorService, WorkflowDefinitionManager} from "@elsa-workflows/elsa-workflows-designer";
import {CookiePlugin} from "../plugins/cookie-plugin";

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
        const pluginRegistry = e.detail.pluginRegistry as PluginRegistry;
        pluginRegistry.remove('home');
        pluginRegistry.remove('login');
        pluginRegistry.add('cookie-credentials', CookiePlugin);
    }
}
