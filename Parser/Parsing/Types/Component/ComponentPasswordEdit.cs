namespace Parser
{
    public class ComponentPasswordEdit : Component, IComponentPasswordEdit
    {
        public ComponentPasswordEdit(IDeclarationSource source, string xamlName, IComponentProperty textProperty)
            : base(source, xamlName)
        {
            TextProperty = textProperty;
        }

        public IComponentProperty TextProperty { get; private set; }
        public IObject TextObject { get; private set; }
        public IObjectPropertyString TextObjectProperty { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IObject Object = TextObject;
            IObjectPropertyString ObjectProperty = TextObjectProperty;
            bool IsConnected = TextProperty.ConnectToObjectStringOnly(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            TextObject = Object;
            TextObjectProperty = ObjectProperty;

            TextObjectProperty?.SetIsReadWrite();

            return IsConnected;
        }
    }
}
