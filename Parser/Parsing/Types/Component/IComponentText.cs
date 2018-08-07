namespace Parser
{
    public interface IComponentText : IComponent
    {
        IResource TextResource { get; }
        IObject TextObject { get; }
        IObjectProperty TextObjectProperty { get; }
        IDeclarationSource TextKey { get; }
        string TextAlignment { get; }
        string TextWrapping { get; }
        string TextDecoration { get; }
    }
}
