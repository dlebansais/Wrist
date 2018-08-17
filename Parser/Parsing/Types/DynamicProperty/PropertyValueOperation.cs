namespace Parser
{
    public class PropertyValueOperation : IPropertyValueOperation
    {
        public PropertyValueOperation(IComponentProperty valueProperty)
        {
            ValueProperty = valueProperty;
        }

        public IComponentProperty ValueProperty { get; private set; }
        public IObject ValueObject { get; private set; }
        public IObjectProperty ValueObjectProperty { get; private set; }
        public IDeclarationSource ValueKey { get; private set; }

        public bool Connect(IDomain domain, IDynamic currentDynamic, IObject currentObject)
        {
            IObject Object = ValueObject;
            IObjectProperty ObjectProperty = ValueObjectProperty;
            IDeclarationSource ObjectPropertyKey = ValueKey;
            bool IsConnected = ValueProperty.ConnectToObjectOnly(domain, null, currentObject, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ValueObject = Object;
            ValueObjectProperty = ObjectProperty;
            ValueKey = ObjectPropertyKey;

            ValueObjectProperty?.SetIsRead();

            return IsConnected;
        }
    }
}
