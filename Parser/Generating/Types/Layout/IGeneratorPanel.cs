namespace Parser
{
    public interface IGeneratorPanel : IGeneratorLayoutElement
    {
        IGeneratorLayoutElementCollection Items { get; }
        string Background { get; }
        string MaxWidth { get; }
        string MaxHeight { get; }
    }
}
