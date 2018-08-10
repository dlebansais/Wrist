using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorColorTheme : IGeneratorColorTheme
    {
        public static Dictionary<IColorTheme, IGeneratorColorTheme> GeneratorColorThemeMap { get; } = new Dictionary<IColorTheme, IGeneratorColorTheme>();

        public GeneratorColorTheme(IColorTheme background)
        {
            Name = background.Name;
            Colors = background.Colors;

            GeneratorColorThemeMap.Add(background, this);
        }

        public string Name { get; private set; }
        public Dictionary<IDeclarationSource, string> Colors { get; private set; }

        public void WriteXamlLine(StreamWriter xamlWriter, string line)
        {
            string ColoredLine = ReplaceColors(line);
            xamlWriter.WriteLine(ColoredLine);
        }

        private string ReplaceColors(string content)
        {
            foreach (KeyValuePair<IDeclarationSource, string> Entry in Colors)
                content = content.Replace($"\"{Entry.Key.Name}\"", $"\"#{Entry.Value}#\"");

            foreach (KeyValuePair<IDeclarationSource, string> Entry in Colors)
                content = content.Replace($"\"#{Entry.Value}#\"", $"\"{Entry.Value}\"");

            return content;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
