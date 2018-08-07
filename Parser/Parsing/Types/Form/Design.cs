using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Parser
{
    public class Design : IDesign, IConnectable
    {
        public Design(List<string> fileNames, string xamlName, ResourceDictionary root)
        {
            FileNames = fileNames;
            XamlName = xamlName;
            Root = root;
        }

        public List<string> FileNames { get; private set; }
        public string Name { get { return Path.GetFileNameWithoutExtension(FileNames[0]); } }
        public string XamlName { get; private set; }
        public ResourceDictionary Root { get; private set; }

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
