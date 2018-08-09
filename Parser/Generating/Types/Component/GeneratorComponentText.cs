using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentText : GeneratorComponent, IGeneratorComponentText
    {
        public GeneratorComponentText(IComponentText text)
            : base(text)
        {
            TextKey = text.TextKey;
            TextDecoration = text.TextDecoration;
            BaseText = text;
        }

        private IComponentText BaseText;

        public IGeneratorResource TextResource { get; private set; }
        public IGeneratorObject TextObject { get; private set; }
        public IGeneratorObjectProperty TextObjectProperty { get; private set; }
        public IDeclarationSource TextKey { get; private set; }
        public string TextDecoration { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (TextResource == null && (TextObject == null || TextObjectProperty == null))
            {
                IsConnected = true;

                if (BaseText.TextResource != null)
                {
                    if (GeneratorResource.GeneratorResourceMap.ContainsKey(BaseText.TextResource))
                        TextResource = GeneratorResource.GeneratorResourceMap[BaseText.TextResource];
                }

                else if (BaseText.TextObject != null && BaseText.TextObjectProperty != null)
                {
                    if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseText.TextObject))
                        TextObject = GeneratorObject.GeneratorObjectMap[BaseText.TextObject];
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseText.TextObjectProperty))
                        TextObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseText.TextObjectProperty];
                }
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string AlignmentProperty = (isHorizontalAlignmentStretch ? $" TextAlignment=\"Justify\"" : "");
            string WrappingProperty = ((textWrapping.HasValue && textWrapping.Value == TextWrapping.NoWrap) ? " TextWrapping=\"NoWrap\"" : " TextWrapping=\"Wrap\"");
            string DecorationProperty = (TextDecoration != null ? $" TextDecorations=\"{TextDecoration}\"" : "");
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Text{StyleProperty}}}\"{AlignmentProperty}{WrappingProperty}{DecorationProperty}";
            string Value = GetComponentValue(currentPage, currentObject, TextResource, TextObject, TextObjectProperty, TextKey, false);

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{attachedProperties}{visibilityBinding} Text=\"{Value}\"{Properties}{elementProperties}/>");
        }
    }
}
