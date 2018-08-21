﻿namespace Parser
{
    public class ComponentButton : Component, IComponentButton
    {
        public ComponentButton(IDeclarationSource source, string xamlName, IComponentProperty contentProperty, IComponentEvent beforeEvent, string goToPageName, IComponentEvent afterEvent, IComponentProperty closePopupProperty)
            : base(source, xamlName)
        {
            ContentProperty = contentProperty;
            BeforeEvent = beforeEvent;
            GoToPageName = goToPageName;
            AfterEvent = afterEvent;
            ClosePopupProperty = closePopupProperty;
        }

        public IComponentProperty ContentProperty { get; private set; }
        public IResource ContentResource { get; private set; }
        public IObject ContentObject { get; private set; }
        public IObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IComponentEvent BeforeEvent { get; private set; }
        public string GoToPageName { get; private set; }
        public IPageNavigation GoTo { get; private set; }
        public IComponentEvent AfterEvent { get; private set; }
        public IComponentProperty ClosePopupProperty { get; private set; }
        public IObject ClosePopupObject { get; private set; }
        public IObjectPropertyBoolean ClosePopupObjectProperty { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentArea, currentObject, ref IsConnected);
            ConnectGoTo(domain, ref IsConnected);
            ConnectClosePopup(domain, currentArea, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectContent(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ContentResource;
            IObject Object = ContentObject;
            IObjectProperty ObjectProperty = ContentObjectProperty;
            IDeclarationSource ObjectPropertyKey = ContentKey;
            IsConnected |= ContentProperty.ConnectToResourceOrObject(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ContentResource = Resource;
            ContentObject = Object;
            ContentObjectProperty = ObjectProperty;
            ContentKey = ObjectPropertyKey;

            ContentObjectProperty?.SetIsRead();
        }

        private void ConnectGoTo(IDomain domain, ref bool IsConnected)
        {
            if (GoTo == null)
            {
                GoTo = new PageNavigation(Source, domain, BeforeEvent, GoToPageName, AfterEvent);
                IsConnected = true;
            }
        }

        private void ConnectClosePopup(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            if (ClosePopupProperty != null)
            {
                IObject Object = ClosePopupObject;
                IObjectPropertyBoolean ObjectProperty = ClosePopupObjectProperty;
                IsConnected |= ClosePopupProperty.ConnectToObjectBooleanOnly(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
                ClosePopupObject = Object;
                ClosePopupObjectProperty = ObjectProperty;

                ClosePopupObjectProperty?.SetIsRead();
                ClosePopupObjectProperty?.SetIsClosingPopup();
            }
        }
    }
}
