namespace Parser
{
    public class BinaryOperation : IBinaryOperation
    {
        public BinaryOperation(DynamicOperationTypes type)
        {
            Type = type;
        }

        public DynamicOperationTypes Type { get; private set; }
        public IDynamicOperation Operand1 { get; set; }
        public IDynamicOperation Operand2 { get; set; }

        public void SetOperand1(IDynamicOperation operand)
        {
            Operand1 = operand;
        }

        public void SetOperand2(IDynamicOperation operand)
        {
            Operand2 = operand;
        }

        public bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject)
        {
            bool IsConnected = false;

            IsConnected |= Operand1.Connect(domain, currentDynamic, currentObject);
            IsConnected |= Operand1.Connect(domain, currentDynamic, currentObject);

            return IsConnected;
        }
    }
}
