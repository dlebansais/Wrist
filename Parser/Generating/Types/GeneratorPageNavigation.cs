namespace Parser
{
    public class GeneratorPageNavigation : IGeneratorPageNavigation
    {
        private GeneratorPageNavigation()
        {
        }

        public GeneratorPageNavigation(IPageNavigation goTo)
        {
            if (goTo.BeforeObject != null && goTo.BeforeObjectEvent != null)
            {
                BeforeObject = GeneratorObject.GeneratorObjectMap[goTo.BeforeObject];
                BeforeObjectEvent = GeneratorObjectEvent.GeneratorObjectEventMap[goTo.BeforeObjectEvent];
            }

            if (goTo.GoToPage == Page.CurrentPage)
                GoToPage = GeneratorPage.CurrentPage;
            else if (goTo.GoToPage == Page.AnyPage)
                GoToPage = GeneratorPage.AnyPage;
            else
                GoToPage = GeneratorPage.GeneratorPageMap[goTo.GoToPage];

            if (goTo.AfterObject != null && goTo.AfterObjectEvent != null)
            {
                AfterObject = GeneratorObject.GeneratorObjectMap[goTo.AfterObject];
                AfterObjectEvent = GeneratorObjectEvent.GeneratorObjectEventMap[goTo.AfterObjectEvent];
            }
        }

        public IGeneratorObject BeforeObject { get; private set; }
        public IGeneratorObjectEvent BeforeObjectEvent { get; private set; }
        public IGeneratorPage GoToPage { get; private set; }
        public IGeneratorObject AfterObject { get; private set; }
        public IGeneratorObjectEvent AfterObjectEvent { get; private set; }
        public IGeneratorComponent Source { get; private set; }

        public string EventName
        {
            get
            {
                string Result = "GoTo_";

                if (BeforeObject != null && BeforeObjectEvent != null)
                    Result += $"{BeforeObject.CSharpName}_{BeforeObjectEvent.CSharpName}__";

                if (GoToPage != GeneratorPage.AnyPage)
                    Result += GoToPage.XamlName;
                else
                    Result += "Any";

                if (AfterObject != null && AfterObjectEvent != null)
                    Result += $"__{AfterObject.CSharpName}_{AfterObjectEvent.CSharpName}";

                return Result;
            }
        }

        public bool IsEqual(IGeneratorPageNavigation other, IGeneratorPage currentPage)
        {
            if (BeforeObject != other.BeforeObject)
                return false;

            if (BeforeObjectEvent != other.BeforeObjectEvent)
                return false;

            IGeneratorPage ThisPage = (GoToPage == GeneratorPage.CurrentPage) ? currentPage : GoToPage;
            IGeneratorPage OtherPage = (other.GoToPage == GeneratorPage.CurrentPage) ? currentPage : other.GoToPage;

            if (ThisPage != OtherPage)
                return false;

            if (AfterObject != other.AfterObject)
                return false;

            if (AfterObjectEvent != other.AfterObjectEvent)
                return false;

            return true;
        }

        public IGeneratorPageNavigation CreateCopyForPage(IGeneratorPage currentPage, IGeneratorComponent source)
        {
            GeneratorPageNavigation Result = new GeneratorPageNavigation();
            Result.BeforeObject = BeforeObject;
            Result.BeforeObjectEvent = BeforeObjectEvent;

            if (GoToPage == GeneratorPage.CurrentPage)
                Result.GoToPage = currentPage;
            else
                Result.GoToPage = GoToPage;

            Result.AfterObject = AfterObject;
            Result.AfterObjectEvent = AfterObjectEvent;
            Result.Source = source;

            return Result;
        }

        public override string ToString()
        {
            string BeforeString = BeforeObject != null ? $" ({BeforeObject.CSharpName}.{BeforeObjectEvent.NameSource.Name})" : "";
            string AfterString = AfterObject != null ? $" ({AfterObject.CSharpName}.{AfterObjectEvent.NameSource.Name})" : "";
            return $"{GetType().Name} ->{BeforeString} {GoToPage.Name}{AfterString}";
        }
    }
}
