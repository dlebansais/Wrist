using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    public class RowDefinition : DependencyObject
    {
        public Double MaxHeight { get; set; }
        public Double MinHeight { get; set; }
        public GridLength Height { get; set; }
        public Double ActualHeight { get; set; }
    }
}
