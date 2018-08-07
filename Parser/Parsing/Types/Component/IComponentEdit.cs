namespace Parser
{
    public interface IComponentEdit : IComponent
    {
        IComponentProperty TextProperty { get; }
        IObject TextObject { get; }
        IObjectPropertyString TextObjectProperty { get; }
        bool AcceptsReturn { get; }
        string TextAlignment { get; }
        string TextWrapping { get; }
        string TextDecoration { get; }
        string HorizontalScrollBarVisibility { get; }
        string VerticalScrollBarVisibility { get; }
    }
}
