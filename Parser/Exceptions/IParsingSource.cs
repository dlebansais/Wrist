namespace Parser
{
    public interface IParsingSource
    {
        string FileName { get; }
        string Line { get; }
        int LineIndex { get; }
    }
}
