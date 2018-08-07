using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class Control : FrameworkElement
    {
        public string Background { get; set; }
        public string BorderBrush { get; set; }
        public string BorderThickness { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public string Foreground { get; set; }
        public FontFamily FontFamily { get; set; }
        public Double FontSize { get; set; }
        public string Padding { get; set; }
        public HorizontalAlignment HorizontalContentAlignment { get; set; }
        public VerticalAlignment VerticalContentAlignment { get; set; }
        public Int32 TabIndex { get; set; }
        public ControlTemplate Template { get; set; }
    }
}
