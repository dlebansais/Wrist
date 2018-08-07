using System.Windows.Markup;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("Points")]
    public class PolyBezierSegment : PathSegment
    {
        public PointCollection Points { get; set; } = new PointCollection();
    }
}
