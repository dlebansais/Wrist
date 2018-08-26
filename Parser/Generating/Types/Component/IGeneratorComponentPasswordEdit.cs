namespace Parser
{
    public interface IGeneratorComponentPasswordEdit : IGeneratorComponent, IGeneratorBindableComponent
    {
        IGeneratorObject TextObject { get; }
        IGeneratorObjectPropertyString TextObjectProperty { get; }
        string PasswordChangedEventName { get; }
    }
}
