namespace Parser
{
    public interface IGeneratorTestingOperation
    {
        IGeneratorPage Page { get;  }
        IGeneratorArea Area { get; }
        IGeneratorComponent Component { get; }
        string TestingFileName { get; }
        int LineIndex { get; }
        bool Connect(IGeneratorDomain domain);
    }
}
