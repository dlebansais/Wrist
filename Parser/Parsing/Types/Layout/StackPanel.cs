using System.Windows.Controls;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class StackPanel : Panel, IStackPanel
    {
        public StackPanel()
        {
            Orientation = Orientation.Vertical;
        }

        public Orientation Orientation { get; set; }

        public override ILayoutElement GetClone()
        {
            StackPanel Clone = new StackPanel();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((StackPanel)clone).Orientation = Orientation;
        }
    }
}
