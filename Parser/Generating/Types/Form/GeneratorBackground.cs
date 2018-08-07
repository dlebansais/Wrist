using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorBackground : IGeneratorBackground
    {
        public static Dictionary<IBackground, IGeneratorBackground> GeneratorBackgroundMap { get; } = new Dictionary<IBackground, IGeneratorBackground>();

        public GeneratorBackground(IBackground background)
        {
            Name = background.Name;
            XamlName = background.XamlName;
            Lines = background.Lines;

            GeneratorBackgroundMap.Add(background, this);
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public List<string> Lines { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public void GenerateResource(StreamWriter xamlWriter, IGeneratorColorScheme colorScheme)
        {
            string Indentation = GeneratorLayout.IndentationString(2);

            foreach (string Line in Lines)
            {
                string ModifiedLine;
                if (Line.StartsWith("<ControlTemplate"))
                    ModifiedLine = $"{Indentation}<ControlTemplate x:Key=\"{XamlName}\" {Line.Substring(16)}";
                else
                    ModifiedLine = $"{Indentation}{Line}";

                colorScheme.WriteXamlLine(xamlWriter, ModifiedLine);
            }
        }

        public void Generate(StreamWriter xamlWriter, int indentation, IGeneratorColorScheme colorScheme)
        {
            string s = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Template=\"{{StaticResource {XamlName}}}\"";

            colorScheme.WriteXamlLine(xamlWriter, $"{s}<ContentControl{Properties}/>");
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
