using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Content")]
    public class ToggleButton : ButtonBase
    {
        public Nullable<Boolean> IsChecked { get; set; }
        public Boolean IsThreeState { get; set; }
    }
}
