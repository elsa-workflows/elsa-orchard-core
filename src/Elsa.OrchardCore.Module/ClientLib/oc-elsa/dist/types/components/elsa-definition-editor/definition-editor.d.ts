export declare class DefinitionEditor {
  private readonly definitionEditorService;
  private readonly definitionManager;
  constructor();
  serverUrl: string;
  definitionId?: string;
  render(): any;
  componentDidLoad(): Promise<void>;
}
