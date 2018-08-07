using System;
using Windows.UI;

namespace Windows.UI.Xaml.Media.Animation
{
    public class ColorAnimation : Timeline
    {
        public EasingFunctionBase EasingFunction { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
