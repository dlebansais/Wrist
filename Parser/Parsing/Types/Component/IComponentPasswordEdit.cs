namespace Parser
{
    public interface IComponentPasswordEdit : IComponent, IComponentWithEvent
    {
        IComponentProperty TextProperty { get; }
        IObject TextObject { get; }
        IObjectPropertyString TextObjectProperty { get; }
    }
}
