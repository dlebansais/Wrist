namespace Parser
{
    public class DynamicProperty : IDynamicProperty
    {
        public DynamicProperty(string propertyName, IDynamicOperation rootOperation)
        {
            PropertyName = propertyName;
            RootOperation = rootOperation;
        }

        public string PropertyName { get; private set; }
        public IDynamicOperation RootOperation { get; private set; }
    }
}
