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

        public override bool Connect(IDomain domain, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectIndex(domain, currentObject, ref IsConnected);
            ConnectItems(domain, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectIndex(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = IndexObject;
            IObjectPropertyInteger ObjectProperty = IndexObjectProperty;
            IsConnected |= IndexProperty.ConnectToObjectIntegerOnly(domain, currentObject, ref Object, ref ObjectProperty);
            IndexObject = Object;
            IndexObjectProperty = ObjectProperty;
        }

        private void ConnectItems(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ItemsResource;
            IObject Object = ItemsObject;
            IObjectPropertyStringList ObjectProperty = ItemsObjectProperty;
            IsConnected |= ItemsProperty.ConnectToStringList(domain, currentObject, ref Resource, ref Object, ref ObjectProperty);
            ItemsResource = Resource;
            ItemsObject = Object;
            ItemsObjectProperty = ObjectProperty;
        }
    }
}
