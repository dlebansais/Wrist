using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentImage : GeneratorComponent, IGeneratorComponentImage
    {
        public GeneratorComponentImage(IComponentImage image)
            : base(image)
        {
            Width = image.Width;
            Height = image.Height;
            BaseImage = image;
        }

        private IComponentImage BaseImage;

        public IGeneratorResource SourceResource { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (SourceResource == null)
            {
                IsConnected = true;

                if (GeneratorResource.GeneratorResourceMap.ContainsKey(BaseImage.SourceResource))
                    SourceResource = GeneratorResource.GeneratorResourceMap[BaseImage.SourceResource];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Image{StyleProperty}}}\" Width=\"{Width}\" Height=\"{Height}\"";
            string Value = GetComponentValue(currentPage, currentObject, SourceResource, null, null, null, false);

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<Image{attachedProperties}{visibilityBinding} Source=\"{Value}\"{Properties}{elementProperties}/>");
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}' ({Width}x{Height})";
        }
    }
}
