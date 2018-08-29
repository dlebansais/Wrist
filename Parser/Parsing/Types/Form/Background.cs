using System.Collections.Generic;

namespace Parser
{
    public class Background : IBackground
    {
        public Background(string name, string xamlName, List<string> lines)
        {
            Name = name;
            XamlName = xamlName;
            Lines = lines;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public List<string> Lines { get; private set; }

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
