using System.Collections.Generic;

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

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectIndex(domain, currentArea, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectIndex(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = IndexObject;
            IObjectPropertyIndex ObjectProperty = IndexObjectProperty;
            IsConnected |= IndexProperty.ConnectToObjectIndex(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            IndexObject = Object;
            IndexObjectProperty = ObjectProperty;

            IndexObjectProperty?.SetIsRead();
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName)
        {
            string Key = ComponentText.FormatStyleResourceKey(design.XamlName, styleName);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);
        }
    }
}
