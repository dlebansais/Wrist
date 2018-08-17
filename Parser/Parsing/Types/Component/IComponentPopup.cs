namespace Parser
{
    public interface IComponentPopup : IComponent
    {
        IComponentProperty SourceProperty { get; }
        IResource SourceResource { get; }
        IComponentProperty SourcePressedProperty { get; }
        IResource SourcePressedResource { get; }
        IDeclarationSource AreaSource { get; }
        IArea Area { get; }
        double Width { get; }
        double Height { get; }
    }
}
