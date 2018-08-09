namespace Parser
{
    public interface IControl : ILayoutElement
    {
        string Name { get; set; }
        string Wrapping { get; set; }
        Windows.UI.Xaml.TextWrapping? TextWrapping { get; }
    }
}
