namespace Parser
{
    public interface ITextDecoration : ILayoutElement
    {
        string Text { get; set; }
        string Wrapping { get; set; }
        Windows.UI.Xaml.TextWrapping? TextWrapping { get; }
    }
}
