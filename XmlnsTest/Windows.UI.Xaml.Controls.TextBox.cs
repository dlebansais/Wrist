using System;
using Windows.UI.Xaml;
using Windows.UI.Text;

namespace Windows.UI.Xaml.Controls
{
    public class TextBox : Control
    {
        public Boolean AcceptsReturn { get; set; }
        public String Text { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public Int32 SelectionStart { get; set; }
        public Int32 SelectionLength { get; set; }
        public TextWrapping TextWrapping { get; set; }
        public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; }
        public Int32 MaxLength { get; set; }
        public Nullable<TextDecorations> TextDecorations { get; set; }
        public Boolean IsReadOnly { get; set; }
    }
}
