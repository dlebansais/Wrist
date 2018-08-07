using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Content")]
    public class RepeatButton : ButtonBase
    {
        public Int32 Delay { get; set; }
        public Int32 Interval { get; set; }
    }
}
