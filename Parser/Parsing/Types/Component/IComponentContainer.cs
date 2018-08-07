namespace Parser
{
    public interface IComponentContainer : IComponent
    {
        IObject ItemObject { get; }
        IObjectPropertyItem ItemObjectProperty { get; }
        IObject ItemNestedObject { get; }
        IDeclarationSource AreaSource { get; }
        IArea ItemNestedArea { get; }
    }
}
