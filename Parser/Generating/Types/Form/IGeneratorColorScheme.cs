using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorColorScheme : IGeneratorForm
    {
        string Name { get; }
        Dictionary<IDeclarationSource, string> Colors { get; }
        void WriteXamlLine(StreamWriter xamlWriter, string line);
    }
}
