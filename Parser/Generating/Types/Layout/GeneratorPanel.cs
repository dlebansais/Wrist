using System;
using System.Collections.Generic;

namespace Parser
{
    public abstract class GeneratorPanel : GeneratorLayoutElement, IGeneratorPanel
    {
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
            MaxWidth = panel.MaxWidth;
            MaxHeight = panel.MaxHeight;

            foreach (LayoutElement Element in panel.Items)
                Items.Add(GeneratorLayoutElement.Convert(Element));
        }

        public IGeneratorLayoutElementCollection Items { get; } = new GeneratorLayoutElementCollection();
        public string MaxWidth { get; set; }
        public string MaxHeight { get; set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = false;

            foreach (IGeneratorLayoutElement Item in Items)
                IsConnected |= Item.Connect(domain, components);

            return IsConnected;
        }

        protected string GetPanelProperties(IGeneratorPage currentPage, IGeneratorObject currentObject)
        {
            string Result = "";

            if (!string.IsNullOrEmpty(MaxWidth))
                Result += $" MaxWidth=\"{MaxWidth}\"";

            if (!string.IsNullOrEmpty(MaxHeight))
                Result += $" MaxHeight=\"{MaxHeight}\"";

            return Result;
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Items.Count} item(s)";
        }
    }
}
