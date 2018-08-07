using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System;

namespace Windows.UI.Xaml.Shapes
{
    public class Shape : FrameworkElement
    {
        public Brush Fill { get; set; }
        public Stretch Stretch { get; set; }
        public Brush Stroke { get; set; }
        public Double StrokeThickness { get; set; }
        public PenLineCap StrokeStartLineCap { get; set; }
        public PenLineCap StrokeEndLineCap { get; set; }
        public PenLineJoin StrokeLineJoin { get; set; }
        public Double StrokeMiterLimit { get; set; }
        public DoubleCollection StrokeDashArray { get; set; } = new DoubleCollection();
        public Double StrokeDashOffset { get; set; }
    }
}
