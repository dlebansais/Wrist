using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class HeaderedContentControl : ContentControl
    {
        public object Header { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
    }
}
