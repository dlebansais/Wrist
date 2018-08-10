using System;
using System.Collections.Generic;

namespace Parser
{
    public abstract class GeneratorPanel : GeneratorLayoutElement, IGeneratorPanel
    {
        public IGeneratorLayoutElementCollection Items { get; } = new GeneratorLayoutElementCollection();

        public static GeneratorPanel Convert(IPanel panel)
        {
            if (panel is DockPanel AsDockPanel)
                return new GeneratorDockPanel(AsDockPanel);
            else if (panel is StackPanel AsStackPanel)
                return new GeneratorStackPanel(AsStackPanel);
            else if (panel is Grid AsGrid)
                return new GeneratorGrid(AsGrid);
            else if (panel is StatePanel AsStatePanel)
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
