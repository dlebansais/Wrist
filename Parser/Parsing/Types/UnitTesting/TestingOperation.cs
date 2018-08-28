namespace Parser
{
    public abstract class TestingOperation : ITestingOperation
    {
        public TestingOperation(IDeclarationSource pageName, IDeclarationSource areaName, IDeclarationSource componentName)
        {
            PageName = pageName;
            AreaName = areaName;
            ComponentName = componentName;
            TestingFileName = pageName.Source.FileName;
            LineIndex = pageName.Source.LineIndex;
        }

        public IDeclarationSource PageName { get; private set; }
        public IPage Page { get; private set; }
        public IDeclarationSource AreaName { get; private set; }
        public IArea Area { get; private set; }
        public IDeclarationSource ComponentName { get; private set; }
        public IComponent Component { get; private set; }
        public string TestingFileName { get; private set; }
        public int LineIndex { get; private set; }

        public virtual bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            if (Page == null)
            {
                foreach (IPage Item in domain.Pages)
                    if (Item.Name == PageName.Name)
                    {
                        Page = Item;
                        break;
                    }
                if (Page == null)
                    throw new ParsingException(227, PageName.Source, $"Page '{PageName}' not found.");

                IsConnected = true;
            }

            if (Area == null)
            {
                foreach (IArea Item in domain.Areas)
                    if (Item.Name == AreaName.Name)
                    {
                        Area = Item;
                        break;
                    }
                if (Area == null)
                    throw new ParsingException(228, AreaName.Source, $"Area '{AreaName}' not found.");

                IsConnected = true;
            }

            if (Component == null && Area != null)
            {
                foreach (IComponent Item in Area.Components)
                    if (Item.Source.Name == ComponentName.Name)
                    {
                        Component = Item;
                        break;
                    }
                if (Component == null)
                    throw new ParsingException(229, ComponentName.Source, $"Component '{ComponentName}' not found in area '{Area.Name}'.");

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
