using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class UnitTest : IUnitTest
    {
        public UnitTest(string unitTestFile, List<ITestingOperation> operations)
        {
            Name = Path.GetFileNameWithoutExtension(unitTestFile);
            UnitTestFile = unitTestFile;
            Operations = operations;
        }

        public string Name { get; private set; }
        public string UnitTestFile { get; private set; }
        public List<ITestingOperation> Operations { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            foreach (TestingOperation Operation in Operations)
                IsConnected |= Operation.Connect(domain);

            return IsConnected;
        }
    }
}
