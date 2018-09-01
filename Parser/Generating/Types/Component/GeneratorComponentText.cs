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

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AlignmentProperty = (isHorizontalAlignmentStretch ? $" TextAlignment=\"Justify\"" : "");
            string WrappingProperty = ((textWrapping.HasValue && textWrapping.Value == TextWrapping.NoWrap) ? " TextWrapping=\"NoWrap\"" : " TextWrapping=\"Wrap\"");
            string DecorationProperty = (TextDecoration != null ? $" TextDecorations=\"{TextDecoration}\"" : "");
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"{AlignmentProperty}{WrappingProperty}{DecorationProperty}";
            string Value = GetComponentValue(currentPage, currentObject, TextResource, TextObject, TextObjectProperty, TextKey, false);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{attachedProperties}{visibilityBinding} Text=\"{Value}\"{Properties}{elementProperties}/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentText.FormatStyleResourceKey(design.XamlName, styleName);
        }
    }
}
