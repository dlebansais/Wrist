using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Windows.UI.Xaml
{
    public class FrameworkElement : UIElement
    {
        public Double Height { get; set; }
        public Double Width { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public string Margin { get; set; }
        public Double MinHeight { get; set; }
        public Double MinWidth { get; set; }
        public Double MaxHeight { get; set; }
        public Double MaxWidth { get; set; }
        public Double ActualWidth { get; set; }
        public Double ActualHeight { get; set; }
        public ResourceDictionary Resources { get; set; } = new ResourceDictionary();
        public DependencyObject Parent { get; set; }
        public Boolean IsLoaded { get; set; }
        public Cursor Cursor { get; set; }
        public Boolean IsEnabled { get; set; }
        public String Name { get; set; }
        public object DataContext { get; set; }
        public object Tag { get; set; }
        public Style Style { get; set; }
        public ContextMenu ContextMenu { get; set; }
    }
}
