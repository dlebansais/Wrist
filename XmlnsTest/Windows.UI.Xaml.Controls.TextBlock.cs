using System.Windows.Markup;
using System;
using Windows.UI.Xaml;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Inlines")]
    public class TextBlock : Control
    {
        public String Text { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextWrapping TextWrapping { get; set; }
        public Nullable<TextDecorations> TextDecorations { get; set; }
        public InlineCollection Inlines { get; set; } = new InlineCollection();
    }
}
