using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorBorderDecoration : GeneratorPanel, IGeneratorBorderDecoration
    {
        public GeneratorBorderDecoration(BorderDecoration control)
            : base(control)
        {
            CornerRadius = control.CornerRadius;
            BorderBrush = control.BorderBrush;
            BorderThickness = control.BorderThickness;
        }

        public string CornerRadius { get; set; }
        public string BorderBrush { get; set; }
        public string BorderThickness { get; set; }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string ElementProperties = GetElementProperties(currentPage, currentObject);
            string BorderProperties = GetPanelProperties(currentPage, currentObject);

            if (!string.IsNullOrEmpty(CornerRadius))
                BorderProperties += $" CornerRadius=\"{CornerRadius}\"";

            if (!string.IsNullOrEmpty(BorderBrush))
                BorderProperties += $" BorderBrush=\"{BorderBrush}\"";

            if (!string.IsNullOrEmpty(BorderThickness))
                BorderProperties += $" BorderThickness=\"{BorderThickness}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Border{AttachedProperties}{visibilityBinding}{BorderProperties}{ElementProperties}>");

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, pageList, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, "");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</Border>");
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
