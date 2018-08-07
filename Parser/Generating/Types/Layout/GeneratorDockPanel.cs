using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class GeneratorDockPanel : GeneratorPanel, IGeneratorDockPanel
    {
        #region Dock Attached Property
        public static Dictionary<IGeneratorLayoutElement, Dock> DockTargets = new Dictionary<IGeneratorLayoutElement, Dock>();

        public static string AttachedDockProperty(IGeneratorLayoutElement element)
        {
            if (DockTargets.ContainsKey(element))
                return $" DockPanel.Dock=\"{DockTargets[element]}\"";
            else
                return "";
        }
        #endregion

        public GeneratorDockPanel(IDockPanel panel)
            : base(panel)
        {
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string DockPanelProperties = "";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<DockPanel{AttachedProperties(this)}{visibilityBinding}{DockPanelProperties}{ElementProperties()}>");

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorScheme, xamlWriter, "");

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}</DockPanel>");
        }
    }
}
