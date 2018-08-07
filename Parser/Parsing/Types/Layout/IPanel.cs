using System.Collections.Generic;

namespace Parser
{
    public interface IPanel : ILayoutElement
    {
        ILayoutElementCollection Items { get; }
        void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids);
    }
}
