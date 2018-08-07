using System;
using Windows.UI;

namespace System.Windows.Media.Effects
{
    public class DropShadowEffect : Effect
    {
        public Double BlurRadius { get; set; }
        public Color Color { get; set; }
        public Double Direction { get; set; }
        public Double Opacity { get; set; }
        public Double ShadowDepth { get; set; }
    }
}
