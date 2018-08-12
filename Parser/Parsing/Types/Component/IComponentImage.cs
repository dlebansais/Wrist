namespace Parser
{
    public interface IComponentImage : IComponent
    {
        IComponentProperty SourceProperty { get; }
        IResource SourceResource { get; }
        double Width { get; }
        double Height { get; }
    }
}
