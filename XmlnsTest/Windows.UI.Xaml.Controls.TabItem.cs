using System.Windows.Markup;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class TabItem : ContentControl
    {
        public object Header { get; set; }
        public Boolean HasHeader { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public Boolean IsSelected { get; set; }
        public Boolean IsFocused { get; set; }
        public Brush SelectedBackground { get; set; }
        public Brush SelectedForeground { get; set; }
        public Brush SelectedAccent { get; set; }
    }
}
