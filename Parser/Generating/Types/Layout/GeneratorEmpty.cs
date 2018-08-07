using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorEmpty : GeneratorLayoutElement, IGeneratorEmpty
    {
        public GeneratorEmpty(Empty control)
            : base(control)
        {
        }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            return false;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties(this)}{visibilityBinding}{ElementProperties()}/>");
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
