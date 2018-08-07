using System.Windows.Markup;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class GridSplitterGripper : ContentControl
    {
        public GridResizeDirection ResizeDirection { get; set; }
    }
}
