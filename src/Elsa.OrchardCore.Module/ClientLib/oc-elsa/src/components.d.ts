/* eslint-disable */
/* tslint:disable */
/**
 * This is an autogenerated file created by the Stencil compiler.
 * It contains typing information for all components that exist in this project.
 */
import { HTMLStencilElement, JSXBase } from "@stencil/core/internal";
export namespace Components {
    interface OcElsaDefinitionEditor {
        "definitionId"?: string;
        "serverUrl": string;
    }
    interface OcElsaInstanceViewer {
        "instanceId"?: string;
        "serverUrl": string;
    }
}
declare global {
    interface HTMLOcElsaDefinitionEditorElement extends Components.OcElsaDefinitionEditor, HTMLStencilElement {
    }
    var HTMLOcElsaDefinitionEditorElement: {
        prototype: HTMLOcElsaDefinitionEditorElement;
        new (): HTMLOcElsaDefinitionEditorElement;
    };
    interface HTMLOcElsaInstanceViewerElement extends Components.OcElsaInstanceViewer, HTMLStencilElement {
    }
    var HTMLOcElsaInstanceViewerElement: {
        prototype: HTMLOcElsaInstanceViewerElement;
        new (): HTMLOcElsaInstanceViewerElement;
    };
    interface HTMLElementTagNameMap {
        "oc-elsa-definition-editor": HTMLOcElsaDefinitionEditorElement;
        "oc-elsa-instance-viewer": HTMLOcElsaInstanceViewerElement;
    }
}
declare namespace LocalJSX {
    interface OcElsaDefinitionEditor {
        "definitionId"?: string;
        "serverUrl"?: string;
    }
    interface OcElsaInstanceViewer {
        "instanceId"?: string;
        "serverUrl"?: string;
    }
    interface IntrinsicElements {
        "oc-elsa-definition-editor": OcElsaDefinitionEditor;
        "oc-elsa-instance-viewer": OcElsaInstanceViewer;
    }
}
export { LocalJSX as JSX };
declare module "@stencil/core" {
    export namespace JSX {
        interface IntrinsicElements {
            "oc-elsa-definition-editor": LocalJSX.OcElsaDefinitionEditor & JSXBase.HTMLAttributes<HTMLOcElsaDefinitionEditorElement>;
            "oc-elsa-instance-viewer": LocalJSX.OcElsaInstanceViewer & JSXBase.HTMLAttributes<HTMLOcElsaInstanceViewerElement>;
        }
    }
}
