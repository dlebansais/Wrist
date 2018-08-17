namespace Parser
{
    public interface IGeneratorPropertyValueOperation : IGeneratorDynamicOperation
    {
        IGeneratorObject ValueObject { get; }
        IGeneratorObjectProperty ValueObjectProperty { get; }
        IDeclarationSource ValueKey { get; }
    }
}
