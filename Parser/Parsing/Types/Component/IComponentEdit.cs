namespace Parser
{
    public interface IComponentEdit : IComponent, IComponentWithEvent
    {
        IComponentProperty TextProperty { get; }
        IObject TextObject { get; }
        IObjectPropertyString TextObjectProperty { get; }
        bool AcceptsReturn { get; }
        string TextDecoration { get; }
        string HorizontalScrollBarVisibility { get; }
        string VerticalScrollBarVisibility { get; }
    }
}
