using System.Windows.Markup;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Children")]
    public class WrapPanel : Panel
    {
        public Orientation Orientation { get; set; }
    }
}
