namespace Parser
{
    public interface IGeneratorUnaryOperation : IGeneratorDynamicOperation
    {
        DynamicOperationTypes Type { get; }
        IGeneratorDynamicOperation Operand { get; }
    }
}
