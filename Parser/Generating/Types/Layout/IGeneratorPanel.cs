namespace Parser
{
    public interface IGeneratorPanel : IGeneratorLayoutElement
    {
        IGeneratorLayoutElementCollection Items { get; }
    }
}
