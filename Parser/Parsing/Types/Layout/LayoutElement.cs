using System.Collections.Generic;

namespace Parser
{
    public abstract class LayoutElement : ILayoutElement
    {
        public LayoutElement()
        {
            Source = ParsingSourceStream.GetCurrentSource();
        }

        public IParsingSource Source { get; private set; }
        public string Style { get; set; }
        public string Background { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Margin { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }

        public abstract void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components);
    }
}
