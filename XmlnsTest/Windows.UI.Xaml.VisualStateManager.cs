using System.Collections;
using System.Collections.Generic;

namespace Windows.UI.Xaml
{
    public class VisualStateManager : DependencyObject
    {
        public static readonly DependencyProperty VisualStateGroupsProperty;
        public static IList GetVisualStateGroups(DependencyObject obj) { return new List<object>(); }
        public static void SetVisualStateGroups(DependencyObject obj, IList value) { }
    }
}
