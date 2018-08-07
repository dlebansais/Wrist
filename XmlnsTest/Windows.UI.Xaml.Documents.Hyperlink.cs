using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Documents
{
    [ContentProperty("Inlines")]
    public class Hyperlink : Span
    {
        public Uri NavigateUri { get; set; }
    }
}
