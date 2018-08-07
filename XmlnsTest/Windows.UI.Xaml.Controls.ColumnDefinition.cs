using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class ColumnDefinition : DependencyObject
    {
        public Double MaxWidth { get; set; }
        public Double MinWidth { get; set; }
        public GridLength Width { get; set; }
        public Double ActualWidth { get; set; }
    }
}
