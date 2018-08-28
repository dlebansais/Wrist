﻿using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class Grid : Panel, IGrid
    {
        #region Column Attached Property
        public static Dictionary<ILayoutElement, int> ColumnTargets = new Dictionary<ILayoutElement, int>();

        public static object GetColumn(object target)
        {
            ILayoutElement AsElement = (ILayoutElement)target;

            if (ColumnTargets.ContainsKey(AsElement))
                return ColumnTargets[AsElement];
            else
                return 0;
        }

        public static void SetColumn(object target, object value)
        {
            ILayoutElement AsElement = (ILayoutElement)target;
            int Column;

            if (value is string AsString)
            {
                if (!int.TryParse(AsString, out Column))
                    throw new ParsingException(161, AsElement.Source, $"Unknown Column value '{AsString}'.");
            }
            else
                throw new ParsingException(162, AsElement.Source, "Missing or invalid Column value.");

            if (ColumnTargets.ContainsKey(AsElement))
                throw new ParsingException(163, AsElement.Source, "Column value already specified for this element.");
            else
                ColumnTargets.Add(AsElement, Column);
        }
        #endregion

        #region ColumnSpan Attached Property
        public static Dictionary<ILayoutElement, int> ColumnSpanTargets = new Dictionary<ILayoutElement, int>();

        public static object GetColumnSpan(object target)
        {
            ILayoutElement AsElement = (ILayoutElement)target;

            if (ColumnSpanTargets.ContainsKey(AsElement))
                return ColumnSpanTargets[AsElement];
            else
                return 0;
        }

        public static void SetColumnSpan(object target, object value)
        {
            ILayoutElement AsElement = (ILayoutElement)target;
            int ColumnSpan;

            if (value is string AsString)
            {
                if (!int.TryParse(AsString, out ColumnSpan))
                    throw new ParsingException(187, AsElement.Source, $"Unknown ColumnSpan value '{AsString}'.");
            }
            else
                throw new ParsingException(188, AsElement.Source, "Missing or invalid ColumnSpan value.");

            if (ColumnSpanTargets.ContainsKey(AsElement))
                throw new ParsingException(189, AsElement.Source, "ColumnSpan value already specified for this element.");
            else
                ColumnSpanTargets.Add(AsElement, ColumnSpan);
        }
        #endregion

        #region Row Attached Property
        public static Dictionary<ILayoutElement, int> RowTargets = new Dictionary<ILayoutElement, int>();

        public static object GetRow(object target)
        {
            ILayoutElement AsElement = (ILayoutElement)target;

            if (RowTargets.ContainsKey(AsElement))
                return RowTargets[AsElement];
            else
                return 0;
        }

        public static void SetRow(object target, object value)
        {
            ILayoutElement AsElement = (ILayoutElement)target;
            int Row;

            if (value is string AsString)
            {
                if (!int.TryParse(AsString, out Row))
                    throw new ParsingException(164, AsElement.Source, $"Unknown Row value '{AsString}'.");
            }
            else
                throw new ParsingException(165, AsElement.Source, "Missing or invalid Row value.");

            if (RowTargets.ContainsKey(AsElement))
                throw new ParsingException(166, AsElement.Source, "Row value already specified for this element.");
            else
                RowTargets.Add(AsElement, Row);
        }
        #endregion

        #region RowSpan Attached Property
        public static Dictionary<ILayoutElement, int> RowSpanTargets = new Dictionary<ILayoutElement, int>();

        public static object GetRowSpan(object target)
        {
            ILayoutElement AsElement = (ILayoutElement)target;

            if (RowSpanTargets.ContainsKey(AsElement))
                return RowSpanTargets[AsElement];
            else
                return 0;
        }

        public static void SetRowSpan(object target, object value)
        {
            ILayoutElement AsElement = (ILayoutElement)target;
            int RowSpan;

            if (value is string AsString)
            {
                if (!int.TryParse(AsString, out RowSpan))
                    throw new ParsingException(190, AsElement.Source, $"Unknown RowSpan value '{AsString}'.");
            }
            else
                throw new ParsingException(191, AsElement.Source, "Missing or invalid RowSpan value.");

            if (RowSpanTargets.ContainsKey(AsElement))
                throw new ParsingException(192, AsElement.Source, "RowSpan value already specified for this element.");
            else
                RowSpanTargets.Add(AsElement, RowSpan);
        }
        #endregion

        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public string ColumnWidths { get; set; }
        public string RowHeights { get; set; }
        public double[] ColumnWidthArray { get; private set; }
        public double[] RowHeightArray { get; private set; }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (ColumnCount < 0)
                throw new ParsingException(167, Source, $"Invalid column count.");
            else if (ColumnCount == 0)
                ColumnCount = 1;

            if (RowCount < 0)
                throw new ParsingException(168, Source, $"Invalid row count.");
            else if (RowCount == 0)
                RowCount = 1;

            ColumnWidthArray = ParseDoubleList(ColumnWidths, ColumnCount, "column width");
            RowHeightArray = ParseDoubleList(RowHeights, RowCount, "row height");
        }

        private double[] ParseDoubleList(string stringList, int maxCount, string propertyName)
        {
            if (stringList == null)
                stringList = "";

            string[] Splitted = stringList.Split(',');
            if (Splitted.Length > maxCount)
                throw new ParsingException(169, Source, $"Too many values in '{stringList}', expected at most {maxCount}.");

            double[] Result = new double[maxCount];

            int i;
            for (i = 0; i < Splitted.Length; i++)
            {
                string WidthString = Splitted[i];
                if (WidthString.Length == 0)
                    Result[i] = 0;
                else if (WidthString.ToLower() == "auto")
                    Result[i] = double.NaN;
                else if (!ParserDomain.TryParseDouble(WidthString, out Result[i]))
                    throw new ParsingException(170, Source, $"'{WidthString}' not parsed as a {propertyName}.");
            }
            for (; i < maxCount; i++)
                Result[i] = 0;

            return Result;
        }

        public override void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            base.ReportElementsWithAttachedProperties(dockPanels, grids);
            grids.Add(this);
        }
    }
}
