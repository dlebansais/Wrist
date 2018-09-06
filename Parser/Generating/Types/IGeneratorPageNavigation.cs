namespace Parser
{
    public interface IGeneratorPageNavigation
    {
        IGeneratorObject BeforeObject { get; }
        IGeneratorObjectEvent BeforeObjectEvent { get; }
        IGeneratorPage GoToPage { get; }
        bool IsExternal { get; }
        IGeneratorObject GoToObject { get; }
        IGeneratorObjectPropertyReadonlyString GoToObjectProperty { get; }
        IGeneratorObject AfterObject { get; }
        IGeneratorObjectEvent AfterObjectEvent { get; }
        bool IsEqual(IGeneratorPageNavigation other, IGeneratorPage currentPage);
        IGeneratorPageNavigation CreateCopyForPage(IGeneratorPage currentPage, IGeneratorComponent source);
        string EventName { get; }
        IGeneratorComponent Source { get; }
    }
}
