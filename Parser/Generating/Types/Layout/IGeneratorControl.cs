namespace Parser
{
    public interface IGeneratorControl : IGeneratorLayoutElement
    {
        string Name { get; }
        Windows.UI.Xaml.TextWrapping? TextWrapping { get; }
    }
}
