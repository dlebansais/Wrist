using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class ListBox : MultiSelector
    {
        public SelectionMode SelectionMode { get; set; }
        public Brush SelectedItemBackgroundBrush { get; set; }
        public Brush SelectedItemForegroundBrush { get; set; }
        public Brush UnselectedItemBackgroundBrush { get; set; }
        public Brush UnselectedItemForegroundBrush { get; set; }
    }
}
