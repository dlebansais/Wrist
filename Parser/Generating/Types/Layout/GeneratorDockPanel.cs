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

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string ElementProperties = GetElementProperties(currentPage, currentObject);
            string DockPanelProperties = GetPanelProperties(currentPage, currentObject);
            string AllProperties = $"{AttachedProperties}{visibilityBinding}{DockPanelProperties}{ElementProperties}";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<DockPanel{AllProperties}>");

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, pageList, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, "");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</DockPanel>");
        }

        public override string GetStyleResourceKey(IGeneratorDesign design)
        {
            return null;
        }
    }
}
