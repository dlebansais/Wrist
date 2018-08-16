namespace Parser
{
    public interface IObjectProperty
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        bool IsRead { get; }
        bool IsWrite { get; }
        bool Connect(IDomain domain);
        void SetIsRead();
        void SetIsWrite();
        void SetIsReadWrite();
    }
}
