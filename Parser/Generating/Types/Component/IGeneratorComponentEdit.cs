namespace Parser
{
    public interface IGeneratorComponentEdit : IGeneratorComponent
    {
        IGeneratorObject TextObject { get; }
        IGeneratorObjectPropertyString TextObjectProperty { get; }
        bool AcceptsReturn { get; }
        string TextDecoration { get; }
        string HorizontalScrollBarVisibility { get; }
        string VerticalScrollBarVisibility { get; }
    }
}
