using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ScrollViewer : ContentControl
    {
        public Double HorizontalOffset { get; set; }
        public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
        public Double VerticalOffset { get; set; }
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; }
    }
}
