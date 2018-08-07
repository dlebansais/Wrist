using System.Windows.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Child")]
    public class Viewbox : ContentControl
    {
        public UIElement Child { get; set; }
        public Stretch Stretch { get; set; }
        public StretchDirection StretchDirection { get; set; }
    }
}
