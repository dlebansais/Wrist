namespace Parser
{
    public class GeneratorDynamicProperty : IGeneratorDynamicProperty
    {
        public GeneratorDynamicProperty(IDynamicProperty dynamicProperty)
        {
            PropertyName = dynamicProperty.PropertyName;
            RootOperation = GeneratorDynamicOperation.Convert(dynamicProperty.RootOperation);
        }

        public string PropertyName { get; private set; }
        public IGeneratorDynamicOperation RootOperation { get; private set; }
    }
}
