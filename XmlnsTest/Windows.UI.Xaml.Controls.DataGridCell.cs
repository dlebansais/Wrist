using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class DataGridCell : ButtonBase
    {
        public DataGridColumn Column { get; set; }
        public Boolean IsEditing { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
