using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class DataGridRow : DependencyObject
    {
        public DataTemplate HeaderTemplate { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
