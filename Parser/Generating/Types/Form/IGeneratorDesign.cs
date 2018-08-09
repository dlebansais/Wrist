using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Parser
{
    public interface IGeneratorDesign : IGeneratorForm
    {
        List<string> FileNames { get; }
        string Name { get; }
        string XamlName { get; }
        ResourceDictionary Root { get; }
        void Generate(IGeneratorDomain domain, string outputFolderName, IGeneratorColorScheme colorScheme);
        void Generate(IGeneratorDomain domain, StreamWriter xamlWriter, bool declareXmlns, int indentation, IGeneratorColorScheme colorScheme);
    }
}
