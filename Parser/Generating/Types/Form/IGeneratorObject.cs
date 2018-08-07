using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorObject : IGeneratorForm
    {
        string Name { get; }
        string CSharpName { get; }
        IReadOnlyCollection<string> States { get; }
        IReadOnlyCollection<IGeneratorObjectProperty> Properties { get; }
        IReadOnlyCollection<IGeneratorTransition> Transitions { get; }
        IReadOnlyCollection<IGeneratorObjectEvent> Events { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorDomain domain, string rootFolderName, string appNamespace);
    }
}
