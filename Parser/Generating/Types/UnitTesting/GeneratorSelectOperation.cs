namespace Parser
{
    public class GeneratorSelectOperation : GeneratorTestingOperation, IGeneratorSelectOperation
    {
        public GeneratorSelectOperation(ISelectOperation operation)
            : base(operation)
        {
            Index = operation.Index;
        }

        public int Index { get; private set; }
    }
}
