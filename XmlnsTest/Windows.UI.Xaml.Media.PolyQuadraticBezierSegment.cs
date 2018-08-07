using System.Windows.Markup;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("Points")]
    public class PolyQuadraticBezierSegment : PathSegment
    {
        public PointCollection Points { get; set; } = new PointCollection();
    }
}
