namespace Parser
{
    public interface IComponentImage : IComponent
    {
        IComponentProperty SourceProperty { get; }
        IResource SourceResource { get; }
        bool IsResourceWidth { get; }
        double Width { get; }
        bool IsResourceHeight { get; }
        double Height { get; }
    }
}
