namespace Parser
{
    public interface IObjectEvent
    {
        string Name { get; }
        string CSharpName { get; }
        bool? IsProvidingCustomPageName { get; }
        void SetIsProvidingCustomPageName(IDeclarationSource componentSource, bool isSet);
    }
}
