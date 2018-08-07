namespace Parser
{
    public interface IDeclarationSource
    {
        string Name { get; }
        IParsingSource Source { get; }
    }
}
