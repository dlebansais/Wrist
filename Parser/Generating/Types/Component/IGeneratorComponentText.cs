namespace Parser
{
    public interface IGeneratorComponentText : IGeneratorComponent
    {
        IGeneratorResource TextResource { get; }
        IGeneratorObject TextObject { get; }
        IGeneratorObjectProperty TextObjectProperty { get; }
        IDeclarationSource TextKey { get; }
        string TextAlignment { get; }
        string TextWrapping { get; }
        string TextDecoration { get; }
    }
}
