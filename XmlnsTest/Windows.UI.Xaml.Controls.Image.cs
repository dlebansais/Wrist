using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    public class Image : FrameworkElement
    {
        public ImageSource Source { get; set; }
        public Stretch Stretch { get; set; }
    }
}
