import App from './App.svelte';

const app = new App({
	target: document.getElementById('workflow-designer-host'),
	props: {
		name: 'world'
	}
});

export default app;