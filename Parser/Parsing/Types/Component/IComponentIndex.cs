namespace Parser
{
    public interface IComponentIndex : IComponent
    {
        IObject IndexObject { get; }
        IObjectPropertyIndex IndexObjectProperty { get; }
    }
}
