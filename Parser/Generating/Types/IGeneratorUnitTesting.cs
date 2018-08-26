using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorUnitTesting
    {
        List<IGeneratorTestingOperation> Operations { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(string outputFolderName, string appNamespace);
    }
}
