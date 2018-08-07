using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class GridSplitter : Control
    {
        public UIElement Element { get; set; }
        public GridResizeDirection ResizeDirection { get; set; }
        public GridResizeBehavior ResizeBehavior { get; set; }
        public Brush GripperForeground { get; set; }
        public Int32 ParentLevel { get; set; }
        public GripperCursorType GripperCursor { get; set; }
        public Int32 GripperCustomCursorResource { get; set; }
        public SplitterCursorBehavior CursorBehavior { get; set; }
    }
}
