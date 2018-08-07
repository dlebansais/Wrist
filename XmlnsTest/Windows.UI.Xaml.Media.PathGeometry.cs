using System.Windows.Markup;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("Figures")]
    public class PathGeometry : Geometry
    {
        public PathFigureCollection Figures { get; set; } = new PathFigureCollection();
        public FillRule FillRule { get; set; }
    }
}
