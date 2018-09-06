using System.Collections.Generic;

namespace Parser
{
    public interface IPageNavigation
    {
        IDeclarationSource NavigationSource { get; }
        IObject BeforeObject { get; }
        IObjectEvent BeforeObjectEvent { get; }
        IPage GoToPage { get; }
        bool IsExternal { get; }
        IObject GoToObject { get; }
        IObjectPropertyReadonlyString GoToObjectProperty { get; }
        IObject AfterObject { get; }
        IObjectEvent AfterObjectEvent { get; }
        IList<IPage> AlternatePages { get; }
    }
}
