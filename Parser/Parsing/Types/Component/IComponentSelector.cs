namespace Parser
{
    public interface IComponentSelector : IComponent
    {
        IObject IndexObject { get; }
        IObjectPropertyIndex IndexObjectProperty { get; }
        IResource ItemsResource { get; }
        IObject ItemsObject { get; }
        IObjectPropertyStringList ItemsObjectProperty { get; }
    }
}
