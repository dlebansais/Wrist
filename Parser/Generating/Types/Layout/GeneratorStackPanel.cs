using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class GeneratorStackPanel : GeneratorPanel, IGeneratorStackPanel
    {
        public GeneratorStackPanel(IStackPanel panel)
            : base(panel)
        {
            Orientation = panel.Orientation;
        }

        public Orientation Orientation { get; private set; }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string StackPanelProperties = GetPanelProperties(currentPage, currentObject);
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            string PanelType;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    PanelType = "WrapPanel";
                    break;

                default:
                case Orientation.Vertical:
                    PanelType = "StackPanel";
                    break;
            }

            string AllProperties = $"{AttachedProperties}{visibilityBinding}{StackPanelProperties}{ElementProperties}";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<{PanelType}{AllProperties}>");

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, "");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</{PanelType}>");
        }
    }
}
