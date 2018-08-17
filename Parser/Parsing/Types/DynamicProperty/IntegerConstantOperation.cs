namespace Parser
{
    public class IntegerConstantOperation : IIntegerConstantOperation
    {
        public IntegerConstantOperation(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject)
        {
            return false;
        }
    }
}
