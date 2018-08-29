namespace Parser
{
    public class Font : IFont
    {
        public Font(string name, string xamlName, string filePath)
        {
            Name = name;
            XamlName = xamlName;
            FilePath = filePath;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FilePath { get; private set; }

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
