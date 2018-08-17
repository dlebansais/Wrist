using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorTextDecoration : GeneratorLayoutElement, IGeneratorTextDecoration
    {
        public GeneratorTextDecoration(TextDecoration control)
            : base(control)
        {
            Text = control.Text;
        }

        public string Text { get; private set; }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string StyleProperty = (Style != null) ? Style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Text{StyleProperty}}}\"";
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{AttachedProperties}{visibilityBinding} Text=\"{Text}\"{Properties}{ElementProperties}/>");
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
