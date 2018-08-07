using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class MenuItem : Button
    {
        public object Icon { get; set; }
        public object Header { get; set; }
    }
}
