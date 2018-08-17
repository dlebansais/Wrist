using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorDynamic : IGeneratorForm
    {
        string Name { get; }
        string FileName { get; }
        string XamlPageName { get; }
        IReadOnlyCollection<IGeneratorDynamicProperty> Properties { get; }
        bool HasProperties { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string outputFolderName, string appNamespace);
    }
}
