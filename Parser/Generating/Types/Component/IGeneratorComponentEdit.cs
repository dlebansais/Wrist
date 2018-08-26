namespace Parser
{
    public interface IGeneratorComponentEdit : IGeneratorComponent, IGeneratorBindableComponent
    {
        IGeneratorObject TextObject { get; }
        IGeneratorObjectPropertyString TextObjectProperty { get; }
        bool AcceptsReturn { get; }
        string TextDecoration { get; }
        string HorizontalScrollBarVisibility { get; }
        string VerticalScrollBarVisibility { get; }
        string TextChangedEventName { get; }
    }
}
