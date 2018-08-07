using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Media.Animation
{
    [ContentProperty("Children")]
    public class Storyboard : Timeline
    {
        public TimelineCollection Children { get; set; } = new TimelineCollection();
        public static readonly DependencyProperty TargetNameProperty;
        public static String GetTargetName(Timeline obj) { return default(String); }
        public static void SetTargetName(Timeline obj, String value) { }
        public static readonly DependencyProperty TargetPropertyProperty;
        public static string GetTargetProperty(Timeline obj) { return default(string); }
        public static void SetTargetProperty(Timeline obj, string value) { }
        public static readonly DependencyProperty TargetProperty;
        public static DependencyObject GetTarget(Timeline obj) { return default(DependencyObject); }
        public static void SetTarget(Timeline obj, DependencyObject value) { }
    }
}
