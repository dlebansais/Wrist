using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorObject : IGeneratorForm
    {
        string Name { get; }
        string CSharpName { get; }
        bool IsGlobal { get; }
        IReadOnlyCollection<IGeneratorObjectProperty> Properties { get; }
        IReadOnlyCollection<IGeneratorObjectEvent> Events { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace);
    }
}
