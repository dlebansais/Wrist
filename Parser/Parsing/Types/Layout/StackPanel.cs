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
    }
}
