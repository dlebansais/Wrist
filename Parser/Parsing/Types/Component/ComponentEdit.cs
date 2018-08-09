namespace Parser
{
    public class ComponentEdit : Component, IComponentEdit
    {
        public ComponentEdit(IDeclarationSource source, string xamlName, IComponentProperty textProperty, bool acceptsReturn, string textAlignment, string textWrapping, string textDecoration, string horizontalScrollBarVisibility, string verticalScrollBarVisibility)
            : base(source, xamlName)
        {
            TextProperty = textProperty;
            AcceptsReturn = acceptsReturn;
            TextAlignment = textAlignment;
            TextWrapping = textWrapping;
            TextDecoration = textDecoration;
            HorizontalScrollBarVisibility = horizontalScrollBarVisibility;
            VerticalScrollBarVisibility = verticalScrollBarVisibility;
        }

        public IComponentProperty TextProperty { get; private set; }
        public IObject TextObject { get; private set; }
        public IObjectPropertyString TextObjectProperty { get; private set; }
        public bool AcceptsReturn { get; private set; }
        public string TextAlignment { get; private set; }
        public string TextWrapping { get; private set; }
        public string TextDecoration { get; private set; }
        public string HorizontalScrollBarVisibility { get; private set; }
        public string VerticalScrollBarVisibility { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IObject currentObject)
        {
            IObject Object = TextObject;
            IObjectPropertyString ObjectProperty = TextObjectProperty;
            bool IsConnected = TextProperty.ConnectToObjectStringOnly(domain, currentObject, ref Object, ref ObjectProperty);
            TextObject = Object;
            TextObjectProperty = ObjectProperty;

            return IsConnected;
        }
    }
}
