namespace Parser
{
    public class UnaryOperation : IUnaryOperation
    {
        public UnaryOperation(DynamicOperationTypes type)
        {
            Type = type;
        }

        public DynamicOperationTypes Type { get; private set; }
        public IDynamicOperation Operand { get; private set; }

        public void SetOperand(IDynamicOperation operand)
        {
            Operand = operand;
        }

        public bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject)
        {
            bool IsConnected = false;

            IsConnected |= Operand.Connect(domain, currentDynamic, currentObject);

            return IsConnected;
        }
    }
}
