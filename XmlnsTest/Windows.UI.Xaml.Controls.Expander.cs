using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class Expander : HeaderedContentControl
    {
        public Boolean IsExpanded { get; set; }
    }
}
