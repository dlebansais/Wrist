using System.Collections.Generic;

namespace Parser
{
    public class Dynamic : IDynamic, IConnectable
    {
        public Dynamic(string name, string fileName, string xamlPageName, IDynamicPropertyCollection properties)
        {
            Name = name;
            FileName = fileName;
            XamlPageName = xamlPageName;
            Properties = properties.AsReadOnly();
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public string XamlPageName { get; private set; }
        public IReadOnlyCollection<IDynamicProperty> Properties { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;

            IObject CurrentObject = null;

            foreach (IObject Obj in domain.Objects)
                if (Obj.Name == Name)
                {
                    CurrentObject = Obj;
                    break;
                }

            foreach (IDynamicProperty Property in Properties)
                IsConnected |= Property.Connect(domain, this, CurrentObject);

            return IsConnected;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
