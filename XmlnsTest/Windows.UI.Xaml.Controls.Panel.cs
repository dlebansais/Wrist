using System.Windows.Markup;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Children")]
    public class Panel : FrameworkElement
    {
        public string Background { get; set; }
        public UIElementCollection Children { get; set; } = new UIElementCollection();
    }
}
