using Windows.UI.Xaml.Controls.Primitives;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class NumericUpDown : RangeBase
    {
        public NumericUpDownValueBarVisibility ValueBarVisibility { get; set; }
        public Boolean IsReadOnly { get; set; }
        public Double DragSpeed { get; set; }
        public Double Increment { get; set; }
    }
}
