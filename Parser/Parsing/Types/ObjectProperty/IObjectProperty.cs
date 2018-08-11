namespace Parser
{
    public interface IObjectProperty
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        bool Connect(IDomain domain);
    }
}
