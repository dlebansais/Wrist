namespace Parser
{
    public interface IGeneratorComponentIndex : IGeneratorComponent
    {
        IGeneratorObject IndexObject { get; }
        IGeneratorObjectPropertyIndex IndexObjectProperty { get; }
        bool IsController { get; }
    }
}
