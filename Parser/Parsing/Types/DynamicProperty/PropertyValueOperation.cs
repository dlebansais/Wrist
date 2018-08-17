namespace Parser
{
    public class PropertyValueOperation : IDynamicOperation
    {
        public PropertyValueOperation(string objectName)
        {
            ObjectName = objectName;
        }

        public string ObjectName { get; private set; }
    }
}
