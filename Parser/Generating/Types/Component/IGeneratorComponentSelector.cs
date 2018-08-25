namespace Parser
{
    public interface IGeneratorComponentSelector : IGeneratorComponent
    {
        IGeneratorObject IndexObject { get; }
        IGeneratorObjectPropertyIndex IndexObjectProperty { get; }
        IGeneratorResource ItemsResource { get; }
        IGeneratorObject ItemsObject { get; }
        IGeneratorObjectPropertyStringList ItemsObjectProperty { get; }
    }
}
