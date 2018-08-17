namespace Parser
{
    public interface IPropertyValueOperation : IDynamicOperation
    {
        IComponentProperty ValueProperty { get; }
        IObject ValueObject { get; }
        IObjectProperty ValueObjectProperty { get; }
        IDeclarationSource ValueKey { get; }
    }
}
