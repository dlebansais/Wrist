using System.Collections.Generic;

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

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentArea, currentObject, ref IsConnected);
            ConnectChecked(domain, currentArea, currentObject, ref IsConnected);

            return IsConnected;
        }

        private void ConnectContent(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ContentResource;
            IObject Object = ContentObject;
            IObjectProperty ObjectProperty = ContentObjectProperty;
            IDeclarationSource ObjectPropertyKey = ContentKey;
            IsConnected |= ContentProperty.ConnectToResourceOrObject(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ContentResource = Resource;
            ContentObject = Object;
            ContentObjectProperty = ObjectProperty;
            ContentKey = ObjectPropertyKey;

            ContentObjectProperty?.SetIsRead();
        }

        private void ConnectChecked(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = CheckedObject;
            IObjectPropertyBoolean ObjectProperty = CheckedObjectProperty;
            IsConnected |= CheckedProperty.ConnectToObjectBoolean(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            CheckedObject = Object;
            CheckedObjectProperty = ObjectProperty;

            CheckedObjectProperty?.SetIsReadWrite();
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName)
        {
            string Key = FormatStyleResourceKey(design.XamlName, styleName);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);
        }

        public static string FormatStyleResourceKey(string xamlDesignName, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{xamlDesignName}CheckBox{StyleProperty}";
        }
    }
}
