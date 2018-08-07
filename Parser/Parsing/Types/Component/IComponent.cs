namespace Parser
{
    public interface IComponent
    {
        IDeclarationSource Source { get; }
        string XamlName { get; }
        bool IsReferencing(IArea other);
        bool Connect(IDomain domain, IObject currentObject);
    }
}
