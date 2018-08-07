using System.Windows.Markup;
using Windows.UI.Xaml;
using Windows.UI;
using System;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("Color")]
    public class GradientStop : DependencyObject
    {
        public Color Color { get; set; }
        public Double Offset { get; set; }
    }
}
