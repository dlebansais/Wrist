using System.Collections.Generic;

namespace Parser
{
    public class ComponentHtml : Component, IComponentHtml
    {
        public ComponentHtml(IDeclarationSource source, string xamlName, IComponentProperty htmlProperty)
            : base(source, xamlName)
        {
            HtmlProperty = htmlProperty;
        }

        public IComponentProperty HtmlProperty { get; private set; }
        public IResource HtmlResource { get; private set; }
        public IObject HtmlObject { get; private set; }
        public IObjectProperty HtmlObjectProperty { get; private set; }
        public IDeclarationSource HtmlKey { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IResource Resource = HtmlResource;
            IObject Object = HtmlObject;
            IObjectProperty ObjectProperty = HtmlObjectProperty;
            IDeclarationSource ObjectPropertyKey = HtmlKey;
            bool IsConnected = HtmlProperty.ConnectToResourceOrObject(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);

            if (!(ObjectProperty is IObjectPropertyReadonlyString) &&
                !(ObjectProperty is IObjectPropertyStringDictionary))
                throw new ParsingException(0, Source.Source, $"Invalid type for property '{Source.Name}'.");

            HtmlResource = Resource;
            HtmlObject = Object;
            HtmlObjectProperty = ObjectProperty;
            HtmlKey = ObjectPropertyKey;

            HtmlObjectProperty?.SetIsRead();

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
            return $"{xamlDesignName}Html{StyleProperty}";
        }
    }
}
