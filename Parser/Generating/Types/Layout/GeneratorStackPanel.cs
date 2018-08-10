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
            string PanelType;
            string StackPanelProperties = "";

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    PanelType = "WrapPanel";
                    //StackPanelProperties = " Orientation=\"Horizontal\"";
                    break;

                default:
                case Orientation.Vertical:
                    PanelType = "StackPanel";
                    //StackPanelProperties = " Orientation=\"Horizontal\"";
                    break;
            }

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<{PanelType}{AttachedProperties(this)}{visibilityBinding}{StackPanelProperties}{ElementProperties()}>");

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, "");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</{PanelType}>");
        }
    }
}
