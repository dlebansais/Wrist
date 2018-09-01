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

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            double LocalWidth = double.IsNaN(Width) ? SourceResource.Width : Width;
            double LocalHeight = double.IsNaN(Height) ? SourceResource.Height : Height;

            string Indentation = GeneratorLayout.IndentationString(indentation);
            string WidthProperty = double.IsNaN(Width) ? "" : $" Width=\"{Width}\"";
            string HeightProperty = double.IsNaN(Height) ? "" : $" Height=\"{Height}\"";
            string StretchProperty = (double.IsNaN(Width) && double.IsNaN(Height)) ? " Stretch=\"Uniform\"" : "";
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"{StretchProperty}{WidthProperty}{HeightProperty}";
            string Value = GetComponentValue(currentPage, currentObject, SourceResource, null, null, null, false);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Image{attachedProperties}{visibilityBinding} Source=\"{Value}\"{Properties}{elementProperties}/>");
        }

        public override string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentImage.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}' ({Width}x{Height})";
        }
    }
}
