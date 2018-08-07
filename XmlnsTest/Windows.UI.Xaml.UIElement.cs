using System;
using System.Windows.Media.Effects;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Windows.UI.Xaml
{
    public class UIElement : DependencyObject
    {
        public Boolean ClipToBounds { get; set; }
        public Effect Effect { get; set; }
        public Transform RenderTransform { get; set; }
        public Point RenderTransformOrigin { get; set; }
        public Boolean UseLayoutRounding { get; set; }
        public Visibility Visibility { get; set; }
        public Double Opacity { get; set; }
        public Boolean IsHitTestVisible { get; set; }
        public Boolean AllowDrop { get; set; }
        public Boolean IsPointerCaptured { get; set; }
    }
}
