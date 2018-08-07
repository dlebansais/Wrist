using System.Windows.Markup;

namespace Windows.UI.Xaml.Documents
{
    [ContentProperty("Inlines")]
    public class Span : Inline
    {
        public InlineCollection Inlines { get; set; } = new InlineCollection();
    }
}
