using System.Windows.Markup;
using Windows.UI.Xaml;
using System;
using Windows.Foundation;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("Segments")]
    public class PathFigure : DependencyObject
    {
        public Boolean IsClosed { get; set; }
        public Boolean IsFilled { get; set; }
        public PathSegmentCollection Segments { get; set; } = new PathSegmentCollection();
        public Point StartPoint { get; set; }
    }
}
