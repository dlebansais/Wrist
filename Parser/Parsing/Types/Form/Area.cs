using System.Collections.Generic;

namespace Parser
{
    public class Area : IArea, IConnectable
    {
        public Area(string name, string xamlName, IComponentCollection components)
        {
            Name = name;
            XamlName = xamlName;
            Components = components.AsReadOnly();
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public IReadOnlyCollection<IComponent> Components { get; private set; }
        public IObject CurrentObject { get; private set; }

        public bool Connect(IDomain domain)
        {
            bool IsConnected = false;
            IObject CurrentObject = null;

            foreach (IObject Item in domain.Objects)
                if (Item.Name == Name)
                {
                    CurrentObject = Item;
                    break;
                }

            foreach (IComponent Component in Components)
                IsConnected |= Component.Connect(domain, CurrentObject);

            return IsConnected;
        }

        public void SetCurrentObject(IDeclarationSource componentSource, IObject currentObject)
        {
            if (CurrentObject == null && currentObject != null)
                CurrentObject = currentObject;
            else if (CurrentObject != null && currentObject == null)
                throw new ParsingException(componentSource.Source, $"Area {Name} used in two different contexts");
            else if (CurrentObject != null && currentObject != null && CurrentObject != currentObject)
                throw new ParsingException(componentSource.Source, $"Area {Name} used for more than one object");
        }

        public bool IsReferencedBy(IArea other)
        {
            if (other == this)
                return true;

            foreach (IComponent component in other.Components)
                if (component.IsReferencing(this))
                    return true;

            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
