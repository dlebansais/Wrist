namespace Parser
{
    public class ComponentButton : Component, IComponentButton
    {
        public ComponentButton(IDeclarationSource source, string xamlName, IComponentProperty contentProperty, IComponentEvent beforeEvent, string goToPageName, IComponentEvent afterEvent)
            : base(source, xamlName)
        {
            ContentProperty = contentProperty;
            BeforeEvent = beforeEvent;
            GoToPageName = goToPageName;
            AfterEvent = afterEvent;
        }

        public IComponentProperty ContentProperty { get; private set; }
        public IResource ContentResource { get; private set; }
        public IObject ContentObject { get; private set; }
        public IObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IComponentEvent BeforeEvent { get; private set; }
        public string GoToPageName { get; private set; }
        public IComponentEvent AfterEvent { get; private set; }
        public IPageNavigation GoTo { get; private set; }

        public override bool Connect(IDomain domain, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentObject, ref IsConnected);
            ConnectGoTo(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectContent(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ContentResource;
            IObject Object = ContentObject;
            IObjectProperty ObjectProperty = ContentObjectProperty;
            IDeclarationSource ObjectPropertyKey = ContentKey;
            IsConnected |= ContentProperty.ConnectToResourceOrObject(domain, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ContentResource = Resource;
            ContentObject = Object;
            ContentObjectProperty = ObjectProperty;
            ContentKey = ObjectPropertyKey;
        }

        private void ConnectGoTo(IDomain domain, ref bool IsConnected)
        {
            if (GoTo == null)
            {
                GoTo = new PageNavigation(Source, domain, BeforeEvent, GoToPageName, AfterEvent);
                IsConnected = true;
            }
        }
    }
}
