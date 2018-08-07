using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Media.Animation
{
    public class Timeline : DependencyObject
    {
        public string Duration { get; set; }
        public RepeatBehavior RepeatBehavior { get; set; }
        public Nullable<TimeSpan> BeginTime { get; set; }
    }
}
