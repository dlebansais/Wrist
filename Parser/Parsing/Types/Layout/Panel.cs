﻿using System.Collections.Generic;

namespace Parser
{
    public abstract class Panel : LayoutElement, IPanel
    {
        public ILayoutElementCollection Items { get; private set; } = new LayoutElementCollection();
        public string Background { get; set; }
        public string MaxWidth { get; set; }
        public string MaxHeight { get; set; }

        public override string FriendlyName { get { return GetType().Name; } }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((Panel)clone).Items = Items.GetClone();
            ((Panel)clone).Background = Background;
            ((Panel)clone).MaxWidth = MaxWidth;
            ((Panel)clone).MaxHeight = MaxHeight;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Items.Count < 1)
                throw new ParsingException(203, Source, $"Panel '{GetType().Name}' has no item.");

            double MaxWidthValue;
            if (MaxWidth != null && !ParserDomain.TryParseDouble(MaxWidth, out MaxWidthValue))
                throw new ParsingException(204, Source, "Invalid max width.");

            double MaxHeightValue;
            if (MaxHeight != null && !ParserDomain.TryParseDouble(MaxHeight, out MaxHeightValue))
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

        public virtual void ReportControlsUsingComponent(List<IControl> controlList, IComponentWithEvent component)
        {
            foreach (ILayoutElement Item in Items)
                if (Item is IPanel AsPanel)
                    AsPanel.ReportControlsUsingComponent(controlList, component);
                else if (Item is IControl AsControl)
                    if (AsControl.Component == component)
                        controlList.Add(AsControl);
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList)
        {
            foreach (ILayoutElement Item in Items)
                Item.ReportResourceKeys(design, KeyList);
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Items.Count} item(s)";
        }
    }
}
