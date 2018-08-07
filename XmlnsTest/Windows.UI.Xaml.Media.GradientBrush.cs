using System.Windows.Markup;

namespace Windows.UI.Xaml.Media
{
    [ContentProperty("GradientStops")]
    public class GradientBrush : Brush
    {
        public GradientStopCollection GradientStops { get; set; } = new GradientStopCollection();
        public BrushMappingMode MappingMode { get; set; }
        public GradientSpreadMethod SpreadMethod { get; set; }
    }
}
