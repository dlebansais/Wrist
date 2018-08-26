namespace Parser
{
    public interface IComponentSelector : IComponent, IComponentWithEvent
    {
        IObject IndexObject { get; }
        IObjectPropertyIndex IndexObjectProperty { get; }
        IResource ItemsResource { get; }
        IObject ItemsObject { get; }
        IObjectPropertyStringList ItemsObjectProperty { get; }
    }
}
