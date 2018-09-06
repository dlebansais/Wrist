using System.Collections.Generic;

namespace Parser
{
    public class ComponentEdit : Component, IComponentEdit
    {
        public ComponentEdit(IDeclarationSource source, string xamlName, IComponentProperty textProperty, bool acceptsReturn, string textDecoration, string horizontalScrollBarVisibility, string verticalScrollBarVisibility)
            : base(source, xamlName)
        {
            TextProperty = textProperty;
            AcceptsReturn = acceptsReturn;
            TextDecoration = textDecoration;
            HorizontalScrollBarVisibility = horizontalScrollBarVisibility;
            VerticalScrollBarVisibility = verticalScrollBarVisibility;
        }

        public IComponentProperty TextProperty { get; private set; }
        public IObject TextObject { get; private set; }
        public IObjectPropertyString TextObjectProperty { get; private set; }
        public bool AcceptsReturn { get; private set; }
        public string TextDecoration { get; private set; }
        public string HorizontalScrollBarVisibility { get; private set; }
        public string VerticalScrollBarVisibility { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IObject Object = TextObject;
            IObjectPropertyString ObjectProperty = TextObjectProperty;
            bool IsConnected = TextProperty.ConnectToObjectString(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            TextObject = Object;
            TextObjectProperty = ObjectProperty;

            TextObjectProperty?.SetIsReadWrite();

            return IsConnected;
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
            return $"{xamlDesignName}Edit{StyleProperty}";
        }
    }
}
