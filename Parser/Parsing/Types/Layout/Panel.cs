using System.Collections.Generic;

namespace Parser
{
    public abstract class Panel : LayoutElement, IPanel
    {
        public ILayoutElementCollection Items { get; } = new LayoutElementCollection();

        public override void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
            foreach (ILayoutElement Item in Items)
                Item.ConnectComponents(domain, components);
        }

        public virtual void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            foreach (ILayoutElement Item in Items)
                if (Item is IPanel AsPanel)
                    AsPanel.ReportElementsWithAttachedProperties(dockPanels, grids);
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Items.Count} item(s)";
        }
    }
}
