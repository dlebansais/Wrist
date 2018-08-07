namespace Parser
{
    public interface IGeneratorComponentContainer : IGeneratorComponent
    {
        IGeneratorObject ItemObject { get; }
        IGeneratorObjectPropertyItem ItemObjectProperty { get; }
        IGeneratorObject ItemNestedObject { get; }
        IGeneratorArea ItemNestedArea { get; }
    }
}
