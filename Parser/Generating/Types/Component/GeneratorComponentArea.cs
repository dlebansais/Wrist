using System.IO;

namespace Parser
{
    public class GeneratorComponentArea : GeneratorComponent, IGeneratorComponentArea
    {
        public GeneratorComponentArea(IComponentArea component)
            : base(component)
        {
            BaseArea = component.Area;
        }

        private IArea BaseArea;

        public IGeneratorArea Area { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (Area == null)
            {
                IsConnected = true;
                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BaseArea))
                    Area = GeneratorArea.GeneratorAreaMap[BaseArea];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            Generate(Area, attachedProperties, elementProperties, indentation, colorScheme, xamlWriter, visibilityBinding, double.NaN);
        }

        public static void Generate(IGeneratorArea area, string attachedProperties, string elementProperties, int indentation, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding, double width)
        {
            string s = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Template=\"{{StaticResource {area.XamlName}}}\"";
            string WidthProperty = double.IsNaN(width) ? "" : $" HorizontalAlignment=\"Center\" Width=\"{width}\"";

            colorScheme.WriteXamlLine(xamlWriter, $"{s}<ContentControl{attachedProperties}{visibilityBinding}{Properties}{elementProperties}{WidthProperty}/>");
        }

        public override bool IsReferencing(IGeneratorArea other)
        {
            if (Area == other)
                return true;

            else if (other.IsReferencedBy(Area))
                return true;

            else
                return false;
        }
    }
}
