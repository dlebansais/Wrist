using System.Windows.Markup;
using System;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ChildWindow : ContentControl
    {
        public Boolean HasCloseButton { get; set; }
        public Brush OverlayBrush { get; set; }
        public Double OverlayOpacity { get; set; }
        public object Title { get; set; }
        public Nullable<Boolean> DialogResult { get; set; }
    }
}
