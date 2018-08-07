namespace Parser
{
    public abstract class GeneratorObjectPropertyIndex : GeneratorObjectProperty, IGeneratorObjectPropertyIndex
    {
        public GeneratorObjectPropertyIndex(IObjectPropertyIndex property, IGeneratorObject obj)
            : base(property, obj)
        {
        }
    }
}
