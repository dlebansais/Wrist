using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class TabControl : ItemsControl
    {
        public object SelectedItem { get; set; }
        public Int32 SelectedIndex { get; set; }
        public object SelectedContent { get; set; }
    }
}
