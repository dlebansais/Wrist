using System.IO;

namespace Parser
{
    public interface IGeneratorObjectEvent
    {
        string Name { get; }
        string CSharpName { get; }
        bool IsProvidingCustomPageName { get; }
        void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);
    }
}
