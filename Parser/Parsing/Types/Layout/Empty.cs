using System.Collections.Generic;

namespace Parser
{
    public class Empty : LayoutElement, IEmpty
    {
        public override void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
