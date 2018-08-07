using System.Collections.Generic;

namespace Parser
{
    public interface ILayout : IForm
    {
        string Name { get; }
        string XamlName { get; }
        IPanel Content { get; }
        void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components);
        void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids);
    }
}
