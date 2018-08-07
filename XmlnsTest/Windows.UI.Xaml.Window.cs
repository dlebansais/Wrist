using System.Windows.Markup;
using Windows.Foundation;
using System;

namespace Windows.UI.Xaml
{
    [ContentProperty("Content")]
    public class Window : FrameworkElement
    {
        public Window Current { get; set; }
        public Rect Bounds { get; set; }
        public object Content { get; set; }
    }
}
