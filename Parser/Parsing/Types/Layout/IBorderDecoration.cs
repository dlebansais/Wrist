namespace Parser
{
    public interface IBorderDecoration : IPanel
    {
        string CornerRadius { get; set; }
        string BorderBrush { get; set; }
        string BorderThickness { get; set; }
    }
}
