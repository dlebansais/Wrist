using System.Reflection;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [DefaultMember("Item")]
    public class UIElementCollection : ObservableCollection<UIElement>
    {
    }
}
