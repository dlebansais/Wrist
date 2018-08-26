namespace Parser
{
    public interface ITestingOperation
    {
        IDeclarationSource PageName { get; }
        IPage Page { get;  }
        IDeclarationSource AreaName { get; }
        IArea Area { get; }
        IDeclarationSource ComponentName { get; }
        IComponent Component { get; }
        string TestingFileName { get; }
        int LineIndex { get; }
        bool Connect(IDomain domain);
    }
}
