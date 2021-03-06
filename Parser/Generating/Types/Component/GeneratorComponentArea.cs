﻿using System.IO;
using Windows.UI.Xaml;

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
                Area = GeneratorArea.GeneratorAreaMap[BaseArea];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            Generate(Area, attachedProperties, elementProperties, indentation, colorTheme, xamlWriter, visibilityBinding, double.NaN);
        }

        public static void Generate(IGeneratorArea area, string attachedProperties, string elementProperties, int indentation, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding, double width)
        {
            string s = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Template=\"{{StaticResource {area.XamlName}}}\"";
            string WidthProperty = double.IsNaN(width) ? "" : $" HorizontalAlignment=\"Center\" Width=\"{width}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{s}<ContentControl{attachedProperties}{visibilityBinding}{Properties}{elementProperties}{WidthProperty}/>");
        }

        public override bool IsReferencing(IGeneratorArea other)
        {
            if (Area == other)
                return true;

            else if (Area == GeneratorArea.EmptyArea)
                return false;

            else if (other.IsReferencedBy(Area))
                return true;

            else
                return false;
        }
    }
}
