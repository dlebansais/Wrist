using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class ComboBox : Selector
    {
        public Boolean UseNativeComboBox { get; set; }
        public Boolean IsDropDownOpen { get; set; }
        public Double MaxDropDownHeight { get; set; }
    }
}
