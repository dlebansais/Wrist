namespace Parser
{
    public interface IComponentImage : IComponent
    {
        IResource SourceResource { get; }
        double Width { get; }
        double Height { get; }
    }
}
