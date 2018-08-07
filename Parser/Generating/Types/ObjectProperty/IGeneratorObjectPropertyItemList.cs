namespace Parser
{
    public interface IGeneratorObjectPropertyItemList : IGeneratorObjectProperty
    {
        IGeneratorObject NestedObject { get; }
    }
}
