using System.Collections.Generic;

namespace Parser
{
    public class Control : LayoutElement, IControl
    {
        public string Name { get; set; }

        public override void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
            bool IsFound = false;
            foreach (IComponent Component in components)
                if (Component.Source.Name == Name)
                {
                    IsFound = true;
                    break;
                }

            if (!IsFound)
                throw new ParsingException(Source, $"Control is referencing {Name} but this name doesn't exist");
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
