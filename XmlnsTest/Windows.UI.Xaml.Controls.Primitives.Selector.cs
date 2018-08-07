using System.Windows.Markup;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Items")]
    public class Selector : ItemsControl
    {
        public Int32 SelectedIndex { get; set; }
        public object SelectedItem { get; set; }
        public object SelectedValue { get; set; }
        public String SelectedValuePath { get; set; }
        public Brush SelectedItemBackground { get; set; }
        public Brush SelectedItemForeground { get; set; }
        public Brush UnselectedItemBackground { get; set; }
        public Brush UnselectedItemForeground { get; set; }
    }
}
