using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorEmpty : GeneratorLayoutElement, IGeneratorEmpty
    {
        public GeneratorEmpty(Empty control)
            : base(control)
        {
            Type = control.Type;
        }

        public string Type { get; private set; }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            if (Type == "Button")
            {
                string Properties = $" Style=\"{{StaticResource {GetButtonStyleResourceKey(design)}}}\"";
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Button{AttachedProperties}{visibilityBinding}{Properties}{ElementProperties} Opacity=\"0\"/>");
            }
            else
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties}{visibilityBinding}{ElementProperties}/>");
        }

        public string GetButtonStyleResourceKey(IGeneratorDesign design)
        {
            return ComponentButton.FormatStyleResourceKey(design.XamlName, Style);
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
