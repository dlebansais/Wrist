using System.IO;

namespace Parser
{
    public interface IGeneratorObjectProperty
    {
        IDeclarationSource NameSource { get; }
        string CSharpName { get; }
        IGeneratorObject Object { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, StreamWriter cSharpWriter);
    }
}
