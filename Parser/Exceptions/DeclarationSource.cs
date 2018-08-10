namespace Parser
{
    public class DeclarationSource : IDeclarationSource
    {
        public DeclarationSource(string name, IParsingSourceStream sourceStream)
        {
            Name = name;
            Source = sourceStream?.FreezedPosition();
        }

        public string Name { get; private set; }
        public IParsingSource Source { get; private set; }

        public override string ToString()
        {
            if (Source != null)
                return $"{Name} (declared in {Source.FileName}, line {Source.LineIndex})";
            else
                return Name;
        }
    }
}
