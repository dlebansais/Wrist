using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ContentControl : Control
    {
        public object Content { get; set; }
        public DataTemplate ContentTemplate { get; set; }
    }
}
