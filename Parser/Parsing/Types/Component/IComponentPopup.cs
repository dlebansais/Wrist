namespace Parser
{
    public interface IComponentPopup : IComponent
    {
        IComponentProperty SourceProperty { get; }
        IResource SourceResource { get; }
        IDeclarationSource AreaSource { get; }
        IArea Area { get; }
    }
}
