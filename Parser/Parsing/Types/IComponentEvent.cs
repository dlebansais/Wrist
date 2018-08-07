namespace Parser
{
    public interface IComponentEvent
    {
        IDeclarationSource EventSource { get; }
        IDeclarationSource ObjectSource { get; }
        IDeclarationSource ObjectEventSource { get; }
        bool Connect(IDomain domain, ref IObject Object, ref IObjectEvent ObjectEvent);
    }
}
