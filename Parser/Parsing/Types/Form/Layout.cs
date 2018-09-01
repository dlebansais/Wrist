using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Content")]
    public class Layout : ILayout
    {
        public void SetName(string name, string xamlName, string fileName)
        {
            Name = name;
            XamlName = xamlName;
            FileName = fileName;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FileName { get; private set; }
        public IPanel Content { get; set; } = new DockPanel();

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            Content.ConnectComponents(domain, currentDynamic, components);
        }

        public void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            Content.ReportElementsWithAttachedProperties(dockPanels, grids);
        }

        public void ReportResourceKeys()
        {
            Content.ReportResourceKeys();
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}' with {Content}";
        }
    }
}
