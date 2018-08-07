using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class ContextMenu : MenuBase
    {
        public Boolean IsOpen { get; set; }
        public Double HorizontalOffset { get; set; }
        public Double VerticalOffset { get; set; }
    }
}
