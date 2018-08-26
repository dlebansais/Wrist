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

                if (GeneratorPage.GeneratorPageMap.ContainsKey(BaseOperation.Page))
                    Page = GeneratorPage.GeneratorPageMap[BaseOperation.Page];
            }

            if (Area == null)
            {
                IsConnected = true;

                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BaseOperation.Area))
                    Area = GeneratorArea.GeneratorAreaMap[BaseOperation.Area];
            }

            if (Component == null)
            {
                if (GeneratorComponent.GeneratorComponentMap.ContainsKey(BaseOperation.Component))
                    Component = GeneratorComponent.GeneratorComponentMap[BaseOperation.Component];

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
