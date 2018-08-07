using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Content")]
    public class SelectorItem : ButtonBase
    {
        public Boolean IsSelected { get; set; }
    }
}
