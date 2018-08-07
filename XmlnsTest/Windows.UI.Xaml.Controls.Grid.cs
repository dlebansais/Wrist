using System.Windows.Markup;
using System;
using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Children")]
    public class Grid : Panel
    {
        public ColumnDefinitionCollection ColumnDefinitions { get; set; } = new ColumnDefinitionCollection();
        public RowDefinitionCollection RowDefinitions { get; set; } = new RowDefinitionCollection();
        public static readonly DependencyProperty RowProperty;
        public static Int32 GetRow(UIElement obj) { return default(Int32); }
        public static void SetRow(UIElement obj, Int32 value) { }
        public static readonly DependencyProperty RowSpanProperty;
        public static Int32 GetRowSpan(UIElement obj) { return default(Int32); }
        public static void SetRowSpan(UIElement obj, Int32 value) { }
        public static readonly DependencyProperty ColumnProperty;
        public static Int32 GetColumn(UIElement obj) { return default(Int32); }
        public static void SetColumn(UIElement obj, Int32 value) { }
        public static readonly DependencyProperty ColumnSpanProperty;
        public static Int32 GetColumnSpan(UIElement obj) { return default(Int32); }
        public static void SetColumnSpan(UIElement obj, Int32 value) { }
    }
}
