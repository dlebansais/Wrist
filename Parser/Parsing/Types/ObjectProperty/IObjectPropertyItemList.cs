namespace Parser
{
    public interface IObjectPropertyItemList : IObjectProperty
    {
        IDeclarationSource ObjectSource { get; }
        IObject NestedObject { get; }
    }
}
