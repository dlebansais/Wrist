using Windows.UI.Xaml.Controls;
using System;

namespace Windows.UI.Xaml.Controls.Primitives
{
    public class RangeBase : Control
    {
        public Double LargeChange { get; set; }
        public Double Maximum { get; set; }
        public Double Minimum { get; set; }
        public Double SmallChange { get; set; }
        public Double Value { get; set; }
    }
}
