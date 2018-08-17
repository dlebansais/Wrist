namespace Parser
{
    public class DynamicOperation : IDynamicOperation
    {
        public DynamicOperation(DynamicOperationTypes type)
        {
            Type = type;
        }

        public DynamicOperationTypes Type { get; set; }
        public IDynamicOperation Operand1 { get; set; }
        public IDynamicOperation Operand2 { get; set; }
    }
}
