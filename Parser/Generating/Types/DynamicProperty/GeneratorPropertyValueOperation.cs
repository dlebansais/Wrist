namespace Parser
{
    public class GeneratorPropertyValueOperation : IGeneratorDynamicOperation
    {
        public GeneratorPropertyValueOperation(PropertyValueOperation value)
        {
            ObjectName = value.ObjectName;
        }

        public string ObjectName { get; private set; }
    }
}
