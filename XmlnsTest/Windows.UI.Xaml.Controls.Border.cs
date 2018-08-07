using System.Windows.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Child")]
    public class Border : FrameworkElement
    {
        public UIElement Child { get; set; }
        public string Background { get; set; }
        public string BorderBrush { get; set; }
        public string BorderThickness { get; set; }
        public string CornerRadius { get; set; }
        public string Padding { get; set; }
    }
}
