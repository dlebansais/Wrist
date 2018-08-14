using System.Collections.Generic;

namespace Parser
{
    public interface ILayoutElement
    {
        IParsingSource Source { get; }
        string Style { get; set; }
        string Background { get; set; }
        string Width { get; set; }
        string Height { get; set; }
        string Margin { get; set; }
        string HorizontalAlignment { get; set; }
        string VerticalAlignment { get; set; }
        string ElementEnable { get; set; }
        IComponent ControllerElement { get; }
        void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components);
    }
}
