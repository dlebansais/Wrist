namespace Parser
{
    public class ComponentText : Component, IComponentText
    {
        public ComponentText(IDeclarationSource source, string xamlName, IComponentProperty textProperty, string textDecoration)
            : base(source, xamlName)
        {
            TextProperty = textProperty;
            TextDecoration = textDecoration;
        }

        public IComponentProperty TextProperty { get; private set; }
        public IResource TextResource { get; private set; }
        public IObject TextObject { get; private set; }
        public IObjectProperty TextObjectProperty { get; private set; }
        public IDeclarationSource TextKey { get; private set; }
        public string TextDecoration { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IResource Resource = TextResource;
            IObject Object = TextObject;
            IObjectProperty ObjectProperty = TextObjectProperty;
            IDeclarationSource ObjectPropertyKey = TextKey;
            bool IsConnected = TextProperty.ConnectToResourceOrObject(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);

            if (!(ObjectProperty is IObjectPropertyInteger) &&
                !(ObjectProperty is IObjectPropertyString) &&
                !(ObjectProperty is IObjectPropertyReadonlyString) &&
                !(ObjectProperty is IObjectPropertyState) &&
                !(ObjectProperty is IObjectPropertyStringDictionary))
                throw new ParsingException(Source.Source, $"Invalid type for property {Source.Name}");

            TextResource = Resource;
            TextObject = Object;
            TextObjectProperty = ObjectProperty;
            TextKey = ObjectPropertyKey;

            return IsConnected;
        }
    }
}
