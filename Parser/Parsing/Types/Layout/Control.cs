using System.Collections.Generic;

namespace Parser
{
    public class Decoration : LayoutElement, IDecoration
    {
        public string Text { get; set; }

        public override void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
