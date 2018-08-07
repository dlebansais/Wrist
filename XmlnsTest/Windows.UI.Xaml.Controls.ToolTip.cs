using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ToolTip : ContentControl
    {
        public Boolean IsOpen { get; set; }
        public Double HorizontalOffset { get; set; }
        public Double VerticalOffset { get; set; }
    }
}
