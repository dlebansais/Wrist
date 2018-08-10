using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorBackground : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        List<string> Lines { get; }
        void GenerateResource(StreamWriter xamlWriter, IGeneratorColorTheme colorTheme);
        void Generate(StreamWriter xamlWriter, int indentation, IGeneratorColorTheme colorTheme);
    }
}
