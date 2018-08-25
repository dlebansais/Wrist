namespace Parser
{
    public interface IObjectEvent
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        bool? IsProvidingCustomPageName { get; }
        void SetIsProvidingCustomPageName(IDeclarationSource componentSource, bool isSet);
        bool Connect(IDomain domain);
        bool IsUsed { get; }
        void SetIsUsed();
    }
}
