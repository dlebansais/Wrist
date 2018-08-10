using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentIndex : GeneratorComponent, IGeneratorComponentIndex
    {
        public GeneratorComponentIndex(IComponentIndex selector)
            : base(selector)
        {
            BaseIndex = selector;
        }

        IComponentIndex BaseIndex;

        public IGeneratorObject IndexObject { get; private set; }
        public IGeneratorObjectPropertyIndex IndexObjectProperty { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (IndexObject == null || IndexObjectProperty == null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseIndex.IndexObject))
                    IndexObject = GeneratorObject.GeneratorObjectMap[BaseIndex.IndexObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseIndex.IndexObjectProperty))
                    IndexObjectProperty = (IGeneratorObjectPropertyIndex)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseIndex.IndexObjectProperty];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Text{StyleProperty}}}\"";
            string Value = GetComponentValue(currentPage, currentObject, null, IndexObject, IndexObjectProperty, null, false);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{attachedProperties}{visibilityBinding} Text=\"{Value}\"{Properties}{elementProperties}/>");
        }
    }
}
