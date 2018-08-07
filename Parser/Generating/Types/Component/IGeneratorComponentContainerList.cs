namespace Parser
{
    public interface IGeneratorComponentContainerList : IGeneratorComponent
    {
        IGeneratorObject ItemObject { get; }
        IGeneratorObjectPropertyItemList ItemObjectProperty { get; }
        IGeneratorObject ItemNestedObject { get; }
        IGeneratorArea ItemNestedArea { get; }
    }
}
