using System.Windows.Markup;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class UserControl : Control
    {
        public UIElement Content { get; set; }
    }
}
