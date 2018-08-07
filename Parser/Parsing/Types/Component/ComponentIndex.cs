namespace Parser
{
    public class ComponentIndex : Component, IComponentIndex
    {
        public ComponentIndex(IDeclarationSource source, string xamlName, IComponentProperty indexProperty)
            : base(source, xamlName)
        {
            IndexProperty = indexProperty;
        }

        public IComponentProperty IndexProperty { get; private set; }
        public IObject IndexObject { get; private set; }
        public IObjectPropertyIndex IndexObjectProperty { get; private set; }

        public override bool Connect(IDomain domain, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectIndex(domain, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectIndex(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = IndexObject;
            IObjectPropertyIndex ObjectProperty = IndexObjectProperty;
            IsConnected |= IndexProperty.ConnectToObjectIndexOnly(domain, currentObject, ref Object, ref ObjectProperty);
            IndexObject = Object;
            IndexObjectProperty = ObjectProperty;
        }
    }
}
