namespace Parser
{
    public class GeneratorToggleOperation : GeneratorTestingOperation, IGeneratorToggleOperation
    {
        public GeneratorToggleOperation(IToggleOperation operation)
            : base(operation)
        {
        }
    }
}
