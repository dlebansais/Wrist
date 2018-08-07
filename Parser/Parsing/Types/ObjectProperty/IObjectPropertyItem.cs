namespace Parser
{
    public interface IObjectPropertyItem : IObjectProperty
    {
        IDeclarationSource ObjectSource { get; }
        IObject NestedObject { get; }
    }
}
