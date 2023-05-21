import {Component, Prop, h} from '@stencil/core';
import {Monaco} from "@elsa-workflows/elsa-workflows-designer/dist/types/components/shared/monaco-editor/utils";
import {isNullOrWhitespace} from "@elsa-workflows/elsa-workflows-designer/dist/types/utils";

const win = window as any;
const require = win.require;

@Component({
    tag: 'oc-elsa-wrapper',
    shadow: false,
})
export class ElsaWrapper {

    @Prop({attribute: 'server-url'}) serverUrl: string;

    render() {
        const serverUrl = this.serverUrl;

        return (
            <elsa-shell>
                <div class="absolute" style={{top: '53px', left: '260px', right: '0'}}>
                    <elsa-workflow-toolbar></elsa-workflow-toolbar>
                </div>
                <div class="absolute inset-0" style={{top: '117px', left: '260px'}}>
                    <elsa-studio serverUrl={serverUrl} disableAuth={true}></elsa-studio>
                </div>
            </elsa-shell>
        )
    }

    async initializeMonacoWorker(libPath?: string): Promise<Monaco> {


        if (isNullOrWhitespace(libPath)) {
            // The lib path is not set, which means external code will be responsible for loading the Monaco editor.
            // In this case, spin while we wait for the editor to be loaded.

            // Create a promise that will resolve after the win.monaco variable exists. Do this in a tight loop, checking for win.monaco to exist on each iteration. Within the same iteration, request the next animation frame as to not block the UI.
            // If we don't get win.monaco after 3 seconds, reject the promise.

            return new Promise<Monaco>((resolve, reject) => {
                const startTime = Date.now();
                const timeout = 3000;

                debugger;

                const checkForMonaco = () => {
                    if (win.monaco) {
                        resolve(win.monaco);
                        return;
                    }

                    if (Date.now() - startTime > timeout) {
                        reject('Monaco editor not loaded.');
                        return;
                    }

                    requestAnimationFrame(checkForMonaco);
                };

                checkForMonaco();
            });
        }
    }
}
