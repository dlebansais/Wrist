using System.Collections.Generic;

namespace Parser
{
    public class PageNavigation : IPageNavigation
    {
        public PageNavigation(IDeclarationSource navigationSource, IDomain domain, IComponentEvent beforeEvent, string goToPageName, bool isExternal, IComponentEvent afterEvent)
        {
            NavigationSource = navigationSource;
            IsExternal = isExternal;
            ConnectBefore(domain, beforeEvent);
            ConnectPage(domain, goToPageName);
            ConnectAfter(domain, afterEvent);
        }

        public PageNavigation(IDeclarationSource navigationSource, IDomain domain, IArea currentArea, IObject currentObject, IComponentEvent beforeEvent, IComponentProperty navigateProperty, IComponentEvent afterEvent)
        {
            NavigationSource = navigationSource;
            IsExternal = true;
            ConnectBefore(domain, beforeEvent);
            ConnectPage(domain, currentArea, currentObject, navigateProperty);
            ConnectAfter(domain, afterEvent);
        }

        public IDeclarationSource NavigationSource { get; private set; }
        public IObject BeforeObject { get; private set; }
        public IObjectEvent BeforeObjectEvent { get; private set; }
        public IPage GoToPage { get; private set; }
        public bool IsExternal { get; private set; }
        public IObject GoToObject { get; private set; }
        public IObjectPropertyString GoToObjectProperty { get; private set; }
        public IObject AfterObject { get; private set; }
        public IObjectEvent AfterObjectEvent { get; private set; }
        public IList<IPage> AlternatePages { get; } = new List<IPage>();

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

            else if (GoToPage != null)
                GoToPage.SetIsReachable();

            else
            {
                string[] Splitted = goToPageName.Split(':');
                string SplittedPrefix = Splitted.Length > 0 && Splitted[0].Length > 0 ? Splitted[0] : "";
                string SplittedSuffix = Splitted.Length > 1 && Splitted[1].Length > 0 ? Splitted[1].Substring(Splitted[1].Length - 1) : "";
                string AnyPagePrefix = Page.AnyPage.Name.Substring(0, Page.AnyPage.Name.Length - 1);
                string AnyPageSuffix = Page.AnyPage.Name.Substring(Page.AnyPage.Name.Length - 1);

                if (SplittedPrefix == AnyPagePrefix && SplittedSuffix == AnyPageSuffix)
                {
                    GoToPage = Page.AnyPage;
                    string[] AlternateNames = Splitted[1].Substring(0, Splitted[1].Length - 1).Split(';');

                    foreach (string AlternateName in AlternateNames)
                    {
                        string TrimmedAlternate = AlternateName.Trim();
                        IPage Alternate = null;
                        foreach (IPage Item in domain.Pages)
                            if (Item.Name == TrimmedAlternate)
                            {
                                Alternate = Item;
                                break;
                            }

                        if (Alternate != null)
                        {
                            if (AlternatePages.Contains(Alternate))
                                throw new ParsingException(222, NavigationSource.Source, $"Custom page destination '{TrimmedAlternate}' is repeated.");

                            Alternate.SetIsReachable();
                            AlternatePages.Add(Alternate);
                        }
                        else if (AlternateName == Page.CurrentPage.Name)
                        {
                            if (!AlternatePages.Contains(Page.CurrentPage))
                                AlternatePages.Add(Page.CurrentPage);
                        }
                        else
                            throw new ParsingException(176, NavigationSource.Source, $"Unknown page name '{TrimmedAlternate}'.");
                    }

                    if (AlternatePages.Count == 0)
                        throw new ParsingException(223, NavigationSource.Source, "A custom page must declare at least one destination page.");
                }
            }

            if (BeforeObjectEvent != null)
                BeforeObjectEvent.SetIsProvidingCustomPageName(NavigationSource, GoToPage == Page.AnyPage);

            if (GoToPage == null)
                throw new ParsingException(176, NavigationSource.Source, $"Unknown page name '{goToPageName}'.");
            if (GoToPage == Page.AnyPage && (BeforeObject == null || BeforeObjectEvent == null))
                throw new ParsingException(177, NavigationSource.Source, "A custom page must be set with a 'before' event.");
        }

        private void ConnectPage(IDomain domain, IArea currentArea, IObject currentObject, IComponentProperty navigateProperty)
        {
            IObject Object = GoToObject;
            IObjectPropertyString ObjectProperty = GoToObjectProperty;
            navigateProperty.ConnectToObjectStringOnly(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            GoToObject = Object;
            GoToObjectProperty = ObjectProperty;

            GoToObjectProperty.SetIsRead();
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
            string BeforeString = BeforeObject != null ? $" ({BeforeObject.CSharpName}.{BeforeObjectEvent.NameSource.Name})" : "";
            string AfterString = AfterObject != null ? $" ({AfterObject.CSharpName}.{AfterObjectEvent.NameSource.Name})" : "";
            return $"{GetType().Name} ->{BeforeString} {GoToPage.Name}{AfterString}";
        }
    }
}
