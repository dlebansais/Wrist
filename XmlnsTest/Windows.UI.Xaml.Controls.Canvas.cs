using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Children")]
    public class Canvas : Panel
    {
        public static readonly DependencyProperty LeftProperty;
        public static Double GetLeft(UIElement obj) { return default(Double); }
        public static void SetLeft(UIElement obj, Double value) { }
        public static readonly DependencyProperty TopProperty;
        public static Double GetTop(UIElement obj) { return default(Double); }
        public static void SetTop(UIElement obj, Double value) { }
        public static readonly DependencyProperty ZIndexProperty;
        public static Int32 GetZIndex(UIElement obj) { return default(Int32); }
        public static void SetZIndex(UIElement obj, Int32 value) { }
    }
}
