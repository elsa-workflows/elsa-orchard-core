import {Config} from '@stencil/core';

const CopyPlugin = require("copy-webpack-plugin");
const targetDir = '../../wwwroot/elsa/elsa-studio';

export const config: Config = {
    namespace: 'oc-elsa',
    outputTargets: [
        {
            type: 'dist',
            esmLoaderPath: '../loader',
            dir: targetDir,
            copy: [
                {src: '../node_modules/@elsa-workflows/elsa-workflows-designer/dist/elsa-workflows-designer/elsa-workflows-designer.css', dest: 'elsa-workflows-designer.css'}
            ]
        },
        {
            type: 'dist-custom-elements',
            dir: targetDir,
            copy: [
                {src: '../node_modules/@elsa-workflows/elsa-workflows-designer/dist/elsa-workflows-designer/elsa-workflows-designer.css', dest: 'elsa-workflows-designer.css'}
            ]
        },
        {
            type: 'www',
            serviceWorker: null, // disable service workers
            dir: targetDir,
            copy: [
                {src: '../node_modules/@elsa-workflows/elsa-workflows-designer/dist/elsa-workflows-designer/elsa-workflows-designer.css', dest: 'elsa-workflows-designer.css'}
            ]
        },
    ],
};
