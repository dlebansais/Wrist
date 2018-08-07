namespace Parser
{
    public interface IObjectProperty
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        bool IsEncrypted { get; }
        bool Connect(IDomain domain);
    }
}
