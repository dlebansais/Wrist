namespace Parser
{
    public class DeclarationSource : IDeclarationSource
    {
        public DeclarationSource(string name, IParsingSource source)
        {
            Name = name;
            Source = source;
        }

        public string Name { get; private set; }
        public IParsingSource Source { get; private set; }

        public override string ToString()
        {
            return $"{Name} (declared in {Source.FileName}, line {Source.LineIndex})";
        }
    }
}
