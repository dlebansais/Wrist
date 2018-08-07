using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Media.Animation
{
    public class ObjectKeyFrame : DependencyObject
    {
        public string KeyTime { get; set; }
        public object Value { get; set; }
    }
}
