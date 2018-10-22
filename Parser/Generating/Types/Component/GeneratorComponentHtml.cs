using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentHtml : GeneratorComponent, IGeneratorComponentHtml
    {
        public GeneratorComponentHtml(IComponentHtml html)
            : base(html)
        {
            HtmlKey = html.HtmlKey;
            BaseHtml = html;
        }

        private IComponentHtml BaseHtml;

        public IGeneratorResource HtmlResource { get; private set; }
        public IGeneratorObject HtmlObject { get; private set; }
        public IGeneratorObjectProperty HtmlObjectProperty { get; private set; }
        public IDeclarationSource HtmlKey { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (HtmlResource == null && (HtmlObject == null || HtmlObjectProperty == null))
            {
                IsConnected = true;

                if (BaseHtml.HtmlResource != null)
                {
                    HtmlResource = GeneratorResource.GeneratorResourceMap[BaseHtml.HtmlResource];
                }

                else if (BaseHtml.HtmlObject != null && BaseHtml.HtmlObjectProperty != null)
                {
                    HtmlObject = GeneratorObject.GeneratorObjectMap[BaseHtml.HtmlObject];
                    HtmlObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseHtml.HtmlObjectProperty];
                }
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = "";
            string Value = GetComponentValue(currentPage, currentObject, HtmlResource, HtmlObject, HtmlObjectProperty, HtmlKey, false);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<n:HtmlPresenter{attachedProperties}{visibilityBinding} Html=\"{Value}\"{Properties}{elementProperties}/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentHtml.FormatStyleResourceKey(design.XamlName, styleName);
        }
    }
}
