using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml.Documents
{
    [ContentProperty("Text")]
    public class Run : Inline
    {
        public String Text { get; set; }
    }
}
