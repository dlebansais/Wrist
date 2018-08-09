namespace Parser
{
    public interface IComponentText : IComponent
    {
        IResource TextResource { get; }
        IObject TextObject { get; }
        IObjectProperty TextObjectProperty { get; }
        IDeclarationSource TextKey { get; }
        string TextDecoration { get; }
    }
}
