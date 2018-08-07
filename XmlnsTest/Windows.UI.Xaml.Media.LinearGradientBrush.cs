using System.Windows.Markup;
using Windows.Foundation;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("GradientStops")]
    public class LinearGradientBrush : GradientBrush
    {
        public Point EndPoint { get; set; }
        public Point StartPoint { get; set; }
    }
}
