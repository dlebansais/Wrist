using System.IO;

namespace Parser
{
    public interface IGeneratorFont : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        string FilePath { get; }
        void Generate(IGeneratorDomain domain, string outputFolderName);
    }
}
