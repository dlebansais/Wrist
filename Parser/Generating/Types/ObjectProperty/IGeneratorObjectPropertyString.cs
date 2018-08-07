namespace Parser
{
    public interface IGeneratorObjectPropertyString : IGeneratorObjectProperty
    {
        int MaximumLength { get; }
        ObjectPropertyStringCategory Category { get; }
    }
}
