using System;
using System.Collections.Generic;

namespace Parser
{
    public abstract class GeneratorPanel : GeneratorLayoutElement, IGeneratorPanel
    {
        public IGeneratorLayoutElementCollection Items { get; } = new GeneratorLayoutElementCollection();

        public static GeneratorPanel Convert(IPanel panel)
        {
            DockPanel AsDockPanel;
            StackPanel AsStackPanel;
            Grid AsGrid;
            StatePanel AsStatePanel;

            if ((AsDockPanel = panel as DockPanel) != null)
                return new GeneratorDockPanel(AsDockPanel);
            else if ((AsStackPanel = panel as StackPanel) != null)
                return new GeneratorStackPanel(AsStackPanel);
            else if ((AsGrid = panel as Grid) != null)
                return new GeneratorGrid(AsGrid);
            else if ((AsStatePanel = panel as StatePanel) != null)
                return new GeneratorStatePanel(AsStatePanel);
            else
                throw new InvalidOperationException();
        }

        public GeneratorPanel(IPanel panel)
            : base(panel)
        {
            foreach (LayoutElement Element in panel.Items)
                Items.Add(GeneratorLayoutElement.Convert(Element));
        }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = false;

            foreach (IGeneratorLayoutElement Item in Items)
                IsConnected |= Item.Connect(domain, components);

            return IsConnected;
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Items.Count} item(s)";
        }
    }
}
