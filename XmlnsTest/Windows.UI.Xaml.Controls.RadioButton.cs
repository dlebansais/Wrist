using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class RadioButton : ToggleButton
    {
        public String GroupName { get; set; }
    }
}
