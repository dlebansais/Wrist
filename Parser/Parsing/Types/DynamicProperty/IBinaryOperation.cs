namespace Parser
{
    public interface IBinaryOperation : IDynamicOperation
    {
        DynamicOperationTypes Type { get; }
        IDynamicOperation Operand1 { get; }
        IDynamicOperation Operand2 { get; }
        void SetOperand1(IDynamicOperation operand);
        void SetOperand2(IDynamicOperation operand);
    }
}
