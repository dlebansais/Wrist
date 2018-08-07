namespace Parser
{
    public interface IGeneratorComponentEdit : IGeneratorComponent
    {
        IGeneratorObject TextObject { get; }
        IGeneratorObjectPropertyString TextObjectProperty { get; }
        bool AcceptsReturn { get; }
        string TextAlignment { get; }
        string TextWrapping { get; }
        string TextDecoration { get; }
        string HorizontalScrollBarVisibility { get; }
        string VerticalScrollBarVisibility { get; }
    }
}
