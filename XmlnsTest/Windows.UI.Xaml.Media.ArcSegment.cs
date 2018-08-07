using System;
using Windows.Foundation;

namespace Windows.UI.Xaml.Media
{
    public class ArcSegment : PathSegment
    {
        public Boolean IsLargeArc { get; set; }
        public Point Point { get; set; }
        public Double RotationAngle { get; set; }
        public Size Size { get; set; }
        public SweepDirection SweepDirection { get; set; }
    }
}
