using System.Windows.Markup;

namespace Windows.UI.Xaml
{
    [ContentProperty("ContentPropertyUsefulOnlyDuringTheCompilation")]
    public class FrameworkTemplate : DependencyObject
    {
        public UIElement ContentPropertyUsefulOnlyDuringTheCompilation { get; set; }
    }
}
