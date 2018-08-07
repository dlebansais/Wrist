using System.IO;

namespace Parser
{
    public interface IGeneratorResource : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
        void GenerateResourceLine(StreamWriter xamlWriter);
        void Generate(IGeneratorDomain domain, string rootFolderName);
    }
}
