namespace Parser
{
    public abstract class GeneratorTestingOperation : IGeneratorTestingOperation
    {
        public GeneratorTestingOperation(ITestingOperation operation)
        {
            TestingFileName = operation.TestingFileName;
            LineIndex = operation.LineIndex;
            BaseOperation = operation;
        }

        private ITestingOperation BaseOperation;

        public IGeneratorPage Page { get; private set; }
        public IGeneratorArea Area { get; private set; }
        public IGeneratorComponent Component { get; private set; }
        public string TestingFileName { get; private set; }
        public int LineIndex { get; private set; }

        public virtual bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (Page == null)
            {
                IsConnected = true;
                Page = GeneratorPage.GeneratorPageMap[BaseOperation.Page];
            }

            if (Area == null)
            {
                IsConnected = true;
                Area = GeneratorArea.GeneratorAreaMap[BaseOperation.Area];
            }

            if (Component == null)
            {
                IsConnected = true;
                Component = GeneratorComponent.GeneratorComponentMap[BaseOperation.Component];
            }

            return IsConnected;
        }
    }
}
