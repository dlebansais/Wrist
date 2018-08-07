namespace Parser
{
    public interface IPageNavigation
    {
        IDeclarationSource NavigationSource { get; }
        IObject BeforeObject { get; }
        IObjectEvent BeforeObjectEvent { get; }
        IPage GoToPage { get; }
        IObject AfterObject { get; }
        IObjectEvent AfterObjectEvent { get; }
    }
}
