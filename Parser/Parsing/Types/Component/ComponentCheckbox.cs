namespace Parser
{
    public class ComponentCheckBox : Component, IComponentCheckBox
    {
        public ComponentCheckBox(IDeclarationSource source, string xamlName, IComponentProperty contentProperty, IComponentProperty checkedProperty)
            : base(source, xamlName)
        {
            ContentProperty = contentProperty;
            CheckedProperty = checkedProperty;
        }

        public IComponentProperty ContentProperty { get; private set; }
        public IResource ContentResource { get; private set; }
        public IObject ContentObject { get; private set; }
        public IObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IComponentProperty CheckedProperty { get; private set; }
        public IObject CheckedObject { get; private set; }
        public IObjectPropertyBoolean CheckedObjectProperty { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentObject, ref IsConnected);
            ConnectChecked(domain, currentObject, ref IsConnected);

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

        private void ConnectChecked(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = CheckedObject;
            IObjectPropertyBoolean ObjectProperty = CheckedObjectProperty;
            IsConnected |= CheckedProperty.ConnectToObjectBooleanOnly(domain, currentObject, ref Object, ref ObjectProperty);
            CheckedObject = Object;
            CheckedObjectProperty = ObjectProperty;
        }
    }
}
