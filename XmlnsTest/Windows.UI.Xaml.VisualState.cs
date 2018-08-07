using System.Windows.Markup;
using System;
using Windows.UI.Xaml.Media.Animation;

namespace Windows.UI.Xaml
{
    [ContentProperty("Storyboard")]
    public class VisualState : DependencyObject
    {
        public String Name { get; set; }
        public Storyboard Storyboard { get; set; }
    }
}
