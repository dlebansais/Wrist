using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorDynamic : IGeneratorForm
    {
        string Name { get; }
        string CSharpName { get; }
        IReadOnlyCollection<IGeneratorDynamicProperty> Properties { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace);
    }
}
