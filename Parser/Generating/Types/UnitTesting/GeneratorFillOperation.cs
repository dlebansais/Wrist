namespace Parser
{
    public class GeneratorFillOperation : GeneratorTestingOperation, IGeneratorFillOperation
    {
        public GeneratorFillOperation(IFillOperation operation)
            : base(operation)
        {
            Content = operation.Content;
        }

        public string Content { get; private set; }
    }
}
