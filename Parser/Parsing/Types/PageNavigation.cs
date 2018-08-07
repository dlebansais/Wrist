namespace Parser
{
    public class PageNavigation : IPageNavigation
    {
        public PageNavigation(IDeclarationSource navigationSource, IDomain domain, IComponentEvent beforeEvent, string goToPageName, IComponentEvent afterEvent)
        {
            NavigationSource = navigationSource;
            ConnectBefore(domain, beforeEvent);
            ConnectPage(domain, goToPageName);
            ConnectAfter(domain, afterEvent);
        }

        public IDeclarationSource NavigationSource { get; private set; }
        public IObject BeforeObject { get; private set; }
        public IObjectEvent BeforeObjectEvent { get; private set; }
        public IPage GoToPage { get; private set; }
        public IObject AfterObject { get; private set; }
        public IObjectEvent AfterObjectEvent { get; private set; }

        private void ConnectBefore(IDomain domain, IComponentEvent beforeEvent)
        {
            if (beforeEvent != null)
            {
                IObject Object = null;
                IObjectEvent ObjectEvent = null;
                beforeEvent.Connect(domain, ref Object, ref ObjectEvent);
                BeforeObject = Object;
                BeforeObjectEvent = ObjectEvent;
            }
            else
            {
                BeforeObject = null;
                BeforeObjectEvent = null;
            }
        }

        private void ConnectPage(IDomain domain, string goToPageName)
        {
            foreach (IPage Item in domain.Pages)
                if (Item.Name == goToPageName)
                {
                    GoToPage = Item;
                    break;
                }

            if (GoToPage == null && goToPageName == Page.CurrentPage.Name)
                GoToPage = Page.CurrentPage;
            else if (GoToPage == null && goToPageName == Page.AnyPage.Name)
                GoToPage = Page.AnyPage;

            if (BeforeObjectEvent != null)
                BeforeObjectEvent.SetIsProvidingCustomPageName(NavigationSource, GoToPage == Page.AnyPage);

            if (GoToPage == null)
                throw new ParsingException(NavigationSource.Source, $"Unknown page name: {goToPageName}");
            if (GoToPage == Page.AnyPage && (BeforeObject == null || BeforeObjectEvent == null))
                throw new ParsingException(NavigationSource.Source, "A custom page must be set with a 'before' event");
        }

        private void ConnectAfter(IDomain domain, IComponentEvent afterEvent)
        {
            if (afterEvent != null)
            {
                IObject Object = null;
                IObjectEvent ObjectEvent = null;
                afterEvent.Connect(domain, ref Object, ref ObjectEvent);
                AfterObject = Object;
                AfterObjectEvent = ObjectEvent;
            }
            else
            {
                AfterObject = null;
                AfterObjectEvent = null;
            }
        }

        public override string ToString()
        {
            string BeforeString = BeforeObject != null ? $" ({BeforeObject.CSharpName}.{BeforeObjectEvent.Name})" : "";
            string AfterString = AfterObject != null ? $" ({AfterObject.CSharpName}.{AfterObjectEvent.Name})" : "";
            return $"{GetType().Name} ->{BeforeString} {GoToPage.Name}{AfterString}";
        }
    }
}
