namespace Parser
{
    public interface IGeneratorComponentSelector : IGeneratorComponent
    {
        IGeneratorObject IndexObject { get; }
        IGeneratorObjectPropertyInteger IndexObjectProperty { get; }
        IGeneratorResource ItemsResource { get; }
        IGeneratorObject ItemsObject { get; }
        IGeneratorObjectPropertyStringList ItemsObjectProperty { get; }
    }
}
