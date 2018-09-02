using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class BorderDecoration : Panel, IBorderDecoration
    {
        public string CornerRadius { get; set; }
        public string BorderBrush { get; set; }
        public string BorderThickness { get; set; }

        public override ILayoutElement GetClone()
        {
            BorderDecoration Clone = new BorderDecoration();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((BorderDecoration)clone).CornerRadius = CornerRadius;
            ((BorderDecoration)clone).BorderBrush = BorderBrush;
            ((BorderDecoration)clone).BorderThickness = BorderThickness;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Items.Count != 1)
                throw new ParsingException(207, Source, $"BorderDecoration must have one nested item.");
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
