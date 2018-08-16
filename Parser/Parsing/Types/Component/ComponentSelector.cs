namespace Parser
{
    public class ComponentSelector : Component, IComponentSelector
    {
        public ComponentSelector(IDeclarationSource source, string xamlName, IComponentProperty indexProperty, IComponentProperty itemsProperty)
            : base(source, xamlName)
        {
            IndexProperty = indexProperty;
            ItemsProperty = itemsProperty;
        }

        public IComponentProperty IndexProperty { get; private set; }
        public IObject IndexObject { get; private set; }
        public IObjectPropertyInteger IndexObjectProperty { get; private set; }
        public IComponentProperty ItemsProperty { get; private set; }
        public IResource ItemsResource { get; private set; }
        public IObject ItemsObject { get; private set; }
        public IObjectPropertyStringList ItemsObjectProperty { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectIndex(domain, currentArea, currentObject, ref IsConnected);
            ConnectItems(domain, currentArea, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectIndex(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = IndexObject;
            IObjectPropertyInteger ObjectProperty = IndexObjectProperty;
            IsConnected |= IndexProperty.ConnectToObjectIntegerOnly(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            IndexObject = Object;
            IndexObjectProperty = ObjectProperty;

            IndexObjectProperty?.SetIsReadWrite();
        }

        private void ConnectItems(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ItemsResource;
            IObject Object = ItemsObject;
            IObjectPropertyStringList ObjectProperty = ItemsObjectProperty;
            IsConnected |= ItemsProperty.ConnectToStringList(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty);
            ItemsResource = Resource;
            ItemsObject = Object;
            ItemsObjectProperty = ObjectProperty;

            ItemsObjectProperty?.SetIsRead();
        }
    }
}
