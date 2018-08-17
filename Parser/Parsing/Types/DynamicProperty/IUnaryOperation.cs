namespace Parser
{
    public interface IUnaryOperation : IDynamicOperation
    {
        DynamicOperationTypes Type { get; }
        IDynamicOperation Operand { get; }
        void SetOperand(IDynamicOperation operand);
    }
}
