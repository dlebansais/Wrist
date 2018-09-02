using System.Collections.Generic;

namespace Parser
{
    public interface ILayoutElement
    {
        IParsingSource Source { get; }
        string Style { get; set; }
        string Width { get; set; }
        string Height { get; set; }
        string Margin { get; set; }
        string HorizontalAlignment { get; set; }
        string VerticalAlignment { get; set; }
        string DynamicEnable { get; set; }
        IDynamicProperty DynamicController { get; }
        ILayoutElement GetClone();
        void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components);
        void ReportResourceKeys(IDesign design, List<string> KeyList);
    }
}
