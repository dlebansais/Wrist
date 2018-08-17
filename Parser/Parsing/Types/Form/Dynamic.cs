using System.Collections.Generic;

namespace Parser
{
    public class Dynamic : IDynamic, IConnectable
    {
        public Dynamic(string name, string cSharpName, IDynamicPropertyCollection properties)
        {
            Name = name;
            CSharpName = cSharpName;
            Properties = properties.AsReadOnly();
        }

        public string Name { get; private set; }
        public string CSharpName { get; private set; }
        public IReadOnlyCollection<IDynamicProperty> Properties { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            return IsConnected;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
