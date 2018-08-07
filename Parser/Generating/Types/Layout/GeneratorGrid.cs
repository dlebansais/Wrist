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

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string GridProperties = "";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties(this)}{visibilityBinding}{GridProperties}{ElementProperties()}>");

            if (ColumnCount > 1)
            {
                colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid.ColumnDefinitions>");

                for (int i = 0; i < ColumnCount; i++)
                {
                    string WidthProperty = (double.IsNaN(ColumnWidthArray[i]) ? " Width=\"Auto\"" : (ColumnWidthArray[i] > 0 ? $" Width=\"{ColumnWidthArray[i]}\"" : ""));
                    colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ColumnDefinition{WidthProperty}/>");
                }

                colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid.ColumnDefinitions>");
            }

            if (RowCount > 1)
            {
                colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid.RowDefinitions>");

                for (int i = 0; i < RowCount; i++)
                {
                    string HeightProperty = (double.IsNaN(RowHeightArray[i]) ? " Height=\"Auto\"" : (RowHeightArray[i] > 0 ? $" Height=\"{RowHeightArray[i]}\"" : ""));
                    colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        <RowDefinition{HeightProperty}/>");
                }

                colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid.RowDefinitions>");
            }

            foreach (IGeneratorLayoutElement element in Items)
                element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorScheme, xamlWriter, "");

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}</Grid>");
        }
    }
}
