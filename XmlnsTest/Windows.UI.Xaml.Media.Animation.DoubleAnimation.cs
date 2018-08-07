using System;

namespace Windows.UI.Xaml.Media.Animation
{
    public class DoubleAnimation : Timeline
    {
        public EasingFunctionBase EasingFunction { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
