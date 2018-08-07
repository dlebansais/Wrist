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

        public override bool Connect(IDomain domain, IObject currentObject)
        {
            IObject Object = TextObject;
            IObjectPropertyString ObjectProperty = TextObjectProperty;
            bool IsConnected = TextProperty.ConnectToObjectStringOnly(domain, currentObject, ref Object, ref ObjectProperty);
            TextObject = Object;
            TextObjectProperty = ObjectProperty;

            if (!ObjectProperty.IsEncrypted || ObjectProperty.Category != ObjectPropertyStringCategory.Password)
                throw new ParsingException(Source.Source, "A password edit can only be associated to an encrypted string property of category 'password'");

            return IsConnected;
        }
    }
}
