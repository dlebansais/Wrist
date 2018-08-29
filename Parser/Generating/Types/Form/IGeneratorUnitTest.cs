using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorUnitTest : IGeneratorForm
    {
        List<IGeneratorTestingOperation> Operations { get; }
        bool Connect(IGeneratorDomain domain);
        void Generate(string outputFolderName, string appNamespace);
    }
}
