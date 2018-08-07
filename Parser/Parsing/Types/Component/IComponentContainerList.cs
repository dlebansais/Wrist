namespace Parser
{
    public interface IComponentContainerList : IComponent
    {
        IObject ItemObject { get; }
        IObjectPropertyItemList ItemObjectProperty { get; }
        IObject ItemNestedObject { get; }
        IDeclarationSource AreaSource { get; }
        IArea ItemNestedArea { get; }
    }
}
