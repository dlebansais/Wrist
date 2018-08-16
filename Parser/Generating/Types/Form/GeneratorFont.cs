using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorFont : IGeneratorFont
    {
        public static Dictionary<IFont, IGeneratorFont> GeneratorFontMap { get; } = new Dictionary<IFont, IGeneratorFont>();

        public GeneratorFont(IFont resource)
        {
            Name = resource.Name;
            XamlName = resource.XamlName;
            FilePath = resource.FilePath;

            GeneratorFontMap.Add(resource, this);
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FilePath { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            return false;
        }

        public void Generate(IGeneratorDomain domain, string outputFolderName)
        {
            string FontsFolderName = Path.Combine(outputFolderName, "Fonts");

            if (!Directory.Exists(FontsFolderName))
                Directory.CreateDirectory(FontsFolderName);

            string DestinationFileName = Path.Combine(FontsFolderName, $"{Name}.ttf");
            File.Copy(FilePath, DestinationFileName, true);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
