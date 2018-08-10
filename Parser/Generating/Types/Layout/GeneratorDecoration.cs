using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorDecoration : GeneratorLayoutElement, IGeneratorDecoration
    {
        public GeneratorDecoration(Decoration control)
            : base(control)
        {
            Text = control.Text;
        }

        public string Text { get; private set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            return false;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (Style != null) ? Style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Text{StyleProperty}}}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{AttachedProperties(this)}{visibilityBinding} Text=\"{Text}\"{Properties}{ElementProperties()}/>");
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
