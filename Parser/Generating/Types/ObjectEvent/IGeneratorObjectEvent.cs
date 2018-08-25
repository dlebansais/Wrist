using System.IO;

namespace Parser
{
    public interface IGeneratorObjectEvent
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        bool IsProvidingCustomPageName { get; }
        void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);
    }
}
