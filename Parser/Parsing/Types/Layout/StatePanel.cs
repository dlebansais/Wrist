using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class StatePanel : Panel, IStatePanel
    {
        public string Index { get; set; }

        public override void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, components);

            IComponent FoundComponent = null;
            foreach (IComponent Component in components)
                if (Component.Source.Name == Index)
                {
                    FoundComponent = Component;
                    break;
                }

            if (FoundComponent == null)
                throw new ParsingException(Source, $"StatePanel is referencing {Index} but this index doesn't exist");
            if (!(FoundComponent is IComponentIndex))
                throw new ParsingException(Source, $"StatePanel is referencing {Index} but this component is not an index");
        }

        public override string ToString()
        {
            return $"{GetType().Name} with index '{Index}'";
        }
    }
}
