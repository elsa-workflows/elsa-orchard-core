import {Component, Prop, h} from '@stencil/core';
import {Container, StudioService, WorkflowDefinitionEditorService, WorkflowDefinitionManager} from "@elsa-workflows/elsa-workflows-designer";

@Component({
    tag: 'oc-elsa-definition-editor',
    styleUrl: 'definition-editor.css',
    shadow: false,
})
export class DefinitionEditor {
    private readonly definitionEditorService: WorkflowDefinitionEditorService;
    private readonly definitionManager: WorkflowDefinitionManager;
    
    constructor() {
        this.definitionManager = Container.get(WorkflowDefinitionManager);
        this.definitionEditorService = Container.get(WorkflowDefinitionEditorService);
    }
    
    @Prop({attribute: 'server-url'}) serverUrl: string;
    @Prop({attribute: 'definition-id'}) definitionId?: string;

    render() {
        const serverUrl = this.serverUrl;

        return (<elsa-studio serverUrl={serverUrl} disableAuth={true}></elsa-studio>
        )
    }
    
    async componentDidLoad() {
        const definitionId = this.definitionId;
        const definition = await this.definitionManager.getWorkflow(definitionId, { isLatest: true });
        this.definitionEditorService.show(definition);
    }
}
