using System.Collections.Generic;

namespace Parser
{
    public interface ILayout : IForm, IConnectable
    {
        string Name { get; }
        string XamlName { get; }
        string FileName { get; }
        IPanel Content { get; }
        ILayout GetClone();
        void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components);
        void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids);
        void ReportResourceKeys(IDesign design, List<string> KeyList);
    }
}
