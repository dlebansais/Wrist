using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class GeneratorGrid : GeneratorPanel, IGeneratorGrid
    {
        #region Column Attached Property
        public static Dictionary<IGeneratorLayoutElement, int> ColumnTargets = new Dictionary<IGeneratorLayoutElement, int>();

        public static string AttachedColumnProperty(IGeneratorLayoutElement element)
        {
            if (ColumnTargets.ContainsKey(element))
                return $" Grid.Column=\"{ColumnTargets[element]}\"";
            else
                return "";
        }
        #endregion

        #region ColumnSpan Attached Property
        public static Dictionary<IGeneratorLayoutElement, int> ColumnSpanTargets = new Dictionary<IGeneratorLayoutElement, int>();

        public static string AttachedColumnSpanProperty(IGeneratorLayoutElement element)
        {
            if (ColumnSpanTargets.ContainsKey(element))
                return $" Grid.ColumnSpan=\"{ColumnSpanTargets[element]}\"";
            else
                return "";
        }
        #endregion

        #region Row Attached Property
        public static Dictionary<IGeneratorLayoutElement, int> RowTargets = new Dictionary<IGeneratorLayoutElement, int>();

        public static string AttachedRowProperty(IGeneratorLayoutElement element)
        {
            if (RowTargets.ContainsKey(element))
                return $" Grid.Row=\"{RowTargets[element]}\"";
            else
                return "";
        }
        #endregion

        #region RowSpan Attached Property
        public static Dictionary<IGeneratorLayoutElement, int> RowSpanTargets = new Dictionary<IGeneratorLayoutElement, int>();

        public static string AttachedRowSpanProperty(IGeneratorLayoutElement element)
        {
            if (RowSpanTargets.ContainsKey(element))
                return $" Grid.RowSpan=\"{RowSpanTargets[element]}\"";
            else
                return "";
        }
        #endregion

        public GeneratorGrid(Grid panel)
            : base(panel)
        {
            ColumnCount = panel.ColumnCount;
            RowCount = panel.RowCount;
            ColumnWidthArray = panel.ColumnWidthArray;
            RowHeightArray = panel.RowHeightArray;
        }

        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }
        public double[] ColumnWidthArray { get; private set; }
        public double[] RowHeightArray { get; private set; }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string GridProperties = "";
            string ElementPropertiesString = ElementProperties(currentPage, currentObject);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties(this)}{visibilityBinding}{GridProperties}{ElementPropertiesString}>");

            if (ColumnCount > 1)
            {
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid.ColumnDefinitions>");

                for (int i = 0; i < ColumnCount; i++)
                {
                    string WidthProperty = (double.IsNaN(ColumnWidthArray[i]) ? " Width=\"Auto\"" : (ColumnWidthArray[i] > 0 ? $" Width=\"{ColumnWidthArray[i]}\"" : ""));
                    colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ColumnDefinition{WidthProperty}/>");
                }

                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid.ColumnDefinitions>");
            }

            if (RowCount > 1)
            {
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid.RowDefinitions>");

                for (int i = 0; i < RowCount; i++)
                {
                    string HeightProperty = (double.IsNaN(RowHeightArray[i]) ? " Height=\"Auto\"" : (RowHeightArray[i] > 0 ? $" Height=\"{RowHeightArray[i]}\"" : ""));
                    colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <RowDefinition{HeightProperty}/>");
                }

                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid.RowDefinitions>");
            }

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, "");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</Grid>");
        }
    }
}
