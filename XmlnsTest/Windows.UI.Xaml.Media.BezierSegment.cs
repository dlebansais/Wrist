using Windows.Foundation;

namespace Windows.UI.Xaml.Media
{
    public class BezierSegment : PathSegment
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public Point Point3 { get; set; }
    }
}
