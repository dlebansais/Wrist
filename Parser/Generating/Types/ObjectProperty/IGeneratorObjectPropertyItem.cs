namespace Parser
{
    public interface IGeneratorObjectPropertyItem : IGeneratorObjectProperty
    {
        IGeneratorObject NestedObject { get; }
    }
}
