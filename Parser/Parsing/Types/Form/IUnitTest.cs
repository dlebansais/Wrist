using System.Collections.Generic;

namespace Parser
{
    public interface IUnitTest : IForm, IConnectable
    {
        string Name { get; }
        string UnitTestFile { get; }
        List<ITestingOperation> Operations { get; }
    }
}
