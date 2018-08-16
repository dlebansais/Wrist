namespace Parser
{
    public interface IGeneratorBorderDecoration : IGeneratorPanel
    {
        string CornerRadius { get; set; }
        string BorderBrush { get; set; }
        string BorderThickness { get; set; }
    }
}
