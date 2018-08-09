namespace Parser
{
    public interface IGeneratorComponentText : IGeneratorComponent
    {
        IGeneratorResource TextResource { get; }
        IGeneratorObject TextObject { get; }
        IGeneratorObjectProperty TextObjectProperty { get; }
        IDeclarationSource TextKey { get; }
        string TextDecoration { get; }
    }
}
