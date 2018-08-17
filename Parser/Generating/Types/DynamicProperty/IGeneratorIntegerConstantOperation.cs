namespace Parser
{
    public interface IGeneratorIntegerConstantOperation : IGeneratorDynamicOperation
    {
        int Value { get; }
    }
}
