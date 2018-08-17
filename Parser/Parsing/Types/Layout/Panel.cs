using System.Collections.Generic;

namespace Parser
{
    public abstract class Panel : LayoutElement, IPanel
    {
        public ILayoutElementCollection Items { get; } = new LayoutElementCollection();
        public string Background { get; set; }
        public string MaxWidth { get; set; }
        public string MaxHeight { get; set; }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Items.Count < 1)
                throw new ParsingException(203, Source, $"Panel '{GetType().Name}' has no item.");

            double MaxWidthValue;
            if (MaxWidth != null && !double.TryParse(MaxWidth, out MaxWidthValue))
                throw new ParsingException(204, Source, "Invalid max width.");

            double MaxHeightValue;
            if (MaxHeight != null && !double.TryParse(MaxHeight, out MaxHeightValue))
                throw new ParsingException(205, Source, "Invalid max height.");

            foreach (ILayoutElement Item in Items)
                Item.ConnectComponents(domain, currentDynamic, components);
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
