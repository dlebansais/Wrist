namespace Parser
{
    public interface IComponentPasswordEdit : IComponent
    {
        IComponentProperty TextProperty { get; }
        IObject TextObject { get; }
        IObjectPropertyString TextObjectProperty { get; }
    }
}
