namespace Parser
{
    public class GeneratorClickOperation : GeneratorTestingOperation, IGeneratorClickOperation
    {
        public GeneratorClickOperation(IClickOperation operation)
            : base(operation)
        {
        }
    }
}
