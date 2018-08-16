using System.Collections.Generic;

namespace Parser
{
    public interface IPanel : ILayoutElement
    {
        ILayoutElementCollection Items { get; }
        string Background { get; set; }
        string MaxWidth { get; set; }
        string MaxHeight { get; set; }
        void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids);
    }
}
