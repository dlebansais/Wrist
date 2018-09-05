using System.Collections.Generic;

namespace Parser
{
    public class Empty : LayoutElement, IEmpty
    {
        public string Type { get; set; }

        public override string FriendlyName { get { return GetType().Name; } }

        public override ILayoutElement GetClone()
        {
            Empty Clone = new Empty();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((Empty)clone).Type = Type;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Type != null)
            {
                if (Type != "Button")
                    throw new ParsingException(221, Source, "Invalid Type for Empty component.");
            }
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList)
        {
            if (Type != null)
            {
                if (Type == "Button")
                {
                    string Key = ComponentButton.FormatStyleResourceKey(design.XamlName, Style);
                    if (!KeyList.Contains(Key))
                        KeyList.Add(Key);
                }
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
