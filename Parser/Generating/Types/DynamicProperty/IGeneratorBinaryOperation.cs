namespace Parser
{
    public interface IGeneratorBinaryOperation : IGeneratorDynamicOperation
    {
        DynamicOperationTypes Type { get; }
        IGeneratorDynamicOperation Operand1 { get; }
        IGeneratorDynamicOperation Operand2 { get; }
    }
}
