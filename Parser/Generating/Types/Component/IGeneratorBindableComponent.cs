namespace Parser
{
    public interface IGeneratorBindableComponent
    {
        IGeneratorObject BoundObject { get; }
        IGeneratorObjectProperty BoundObjectProperty { get; }
        string HandlerArgumentTypeName { get; }
        bool PostponeChangedNotification { get; }
    }
}
