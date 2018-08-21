using System.Collections.Generic;

namespace Parser
{
    public class Empty : LayoutElement, IEmpty
    {
        public string Type { get; set; }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Type != null &&
                Type != "Button")
                throw new ParsingException(0, Source, "Invalid Type for Empty component.");
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
