using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Children")]
    public class DockPanel : Panel
    {
        public Boolean LastChildFill { get; set; }
        public static readonly DependencyProperty DockProperty;
        public static Dock GetDock(DependencyObject obj) { return default(Dock); }
        public static void SetDock(DependencyObject obj, Dock value) { }
    }
}
