namespace Parser
{
    public interface IGeneratorComponentPasswordEdit : IGeneratorComponent
    {
        IGeneratorObject TextObject { get; }
        IGeneratorObjectPropertyString TextObjectProperty { get; }
    }
}
