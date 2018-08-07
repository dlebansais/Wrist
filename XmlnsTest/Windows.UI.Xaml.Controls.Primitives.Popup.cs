using System.Windows.Markup;
using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Child")]
    public class Popup : FrameworkElement
    {
        public UIElement PlacementTarget { get; set; }
        public PlacementMode Placement { get; set; }
        public UIElement Child { get; set; }
        public Boolean IsOpen { get; set; }
        public Double HorizontalOffset { get; set; }
        public Double VerticalOffset { get; set; }
        public HorizontalAlignment HorizontalContentAlignment { get; set; }
        public VerticalAlignment VerticalContentAlignment { get; set; }
    }
}
