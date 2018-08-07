using System.Windows.Markup;
using Windows.Foundation;
using System;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("GradientStops")]
    public class RadialGradientBrush : GradientBrush
    {
        public Point Center { get; set; }
        public Point GradientOrigin { get; set; }
        public Double RadiusX { get; set; }
        public Double RadiusY { get; set; }
    }
}
