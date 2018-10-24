using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorColorTheme : IGeneratorColorTheme
    {
        public static Dictionary<IColorTheme, IGeneratorColorTheme> GeneratorColorThemeMap { get; } = new Dictionary<IColorTheme, IGeneratorColorTheme>();

        public GeneratorColorTheme(IColorTheme colorTheme)
        {
            Name = colorTheme.Name;
            Colors = colorTheme.Colors;

            GeneratorColorThemeMap.Add(colorTheme, this);
        }

        public string Name { get; private set; }
        public Dictionary<IDeclarationSource, string> Colors { get; private set; }

        public void WriteXamlLine(StreamWriter xamlWriter, string line)
        {
            string ColoredLine = ReplaceColors(Colors, line);
            xamlWriter.WriteLine(ColoredLine);
        }

        public void WriteXamlLine(StreamWriter xamlWriter, ICollection<IGeneratorFont> fonts, string line)
        {
            string MergedLine = line;

            MergedLine = ReplaceColors(Colors, MergedLine);
            MergedLine = ReplaceFonts(fonts, MergedLine);

            xamlWriter.WriteLine(MergedLine);
        }

        private static string ReplaceColors(Dictionary<IDeclarationSource, string> colors, string content)
        {
            foreach (KeyValuePair<IDeclarationSource, string> Entry in colors)
                content = content.Replace($"\"{Entry.Key.Name}\"", $"\"#{Entry.Value}#\"");

            foreach (KeyValuePair<IDeclarationSource, string> Entry in colors)
                content = content.Replace($"\"#{Entry.Value}#\"", $"\"{Entry.Value}\"");

            return content;
        }

        private static string ReplaceFonts(ICollection<IGeneratorFont> fonts, string content)
        {
            foreach (IGeneratorFont Item in fonts)
                content = content.Replace($"\"{Item.Name}.ttf\"", $"\"#{Item.Name}#\"");

            foreach (IGeneratorFont Item in fonts)
                content = content.Replace($"\"#{Item.Name}#\"", $"\"/AppCSHtml5;component/fonts/{Item.Name.ToLower()}.ttf\"");

            return content;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
