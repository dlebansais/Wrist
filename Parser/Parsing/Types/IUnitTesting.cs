using System.Collections.Generic;

namespace Parser
{
    public interface IUnitTesting : IConnectable
    {
        string UnitTestingFile { get; }
        List<ITestingOperation> Operations { get; }
        void Process();
    }
}
