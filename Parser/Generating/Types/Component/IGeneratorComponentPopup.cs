namespace Parser
{
    public interface IGeneratorComponentPopup : IGeneratorComponent
    {
        IGeneratorResource SourceResource { get; }
        IGeneratorArea Area { get; }
    }
}
