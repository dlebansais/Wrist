namespace Parser
{
    public interface IGeneratorComponentImage : IGeneratorComponent
    {
        IGeneratorResource SourceResource { get; }
        double Width { get; }
        double Height { get; }
    }
}
