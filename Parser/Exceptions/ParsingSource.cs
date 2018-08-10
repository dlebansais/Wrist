namespace Parser
{
    public class ParsingSource : IParsingSource
    {
        public ParsingSource(string fileName, string line, int lineIndex)
        {
            FileName = fileName;
            Line = line;
            LineIndex = lineIndex;
        }

        public string FileName { get; private set; }
        public string Line { get; private set; }
        public int LineIndex { get; private set; }
    }
}
