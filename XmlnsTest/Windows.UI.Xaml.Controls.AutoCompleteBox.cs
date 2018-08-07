using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class AutoCompleteBox : Selector
    {
        public Boolean IsDropDownOpen { get; set; }
        public Boolean IsArrowVisible { get; set; }
        public Int32 MinimumPopulateDelay { get; set; }
        public Int32 MinimumPrefixLength { get; set; }
        public String SearchText { get; set; }
        public String Text { get; set; }
        public AutoCompleteFilterMode FilterMode { get; set; }
        public Double MaxDropDownHeight { get; set; }
    }
}
