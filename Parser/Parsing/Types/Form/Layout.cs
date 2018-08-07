using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Content")]
    public class Layout : ILayout, IConnectable
    {
        public void SetName(string name, string xamlName)
        {
            Name = name;
            XamlName = xamlName;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public IPanel Content { get; set; } = new DockPanel();

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
            Content.ConnectComponents(domain, components);
        }

        public void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            Content.ReportElementsWithAttachedProperties(dockPanels, grids);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}' with {Content}";
        }
    }
}
