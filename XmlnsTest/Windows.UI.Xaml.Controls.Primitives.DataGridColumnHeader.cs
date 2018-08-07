using System.Windows.Markup;
using Windows.UI.Xaml.Controls;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Content")]
    public class DataGridColumnHeader : ButtonBase
    {
        public DataGridColumn Column { get; set; }
    }
}
