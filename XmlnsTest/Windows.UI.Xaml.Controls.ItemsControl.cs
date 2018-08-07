using System.Windows.Markup;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class ItemsControl : Control
    {
        public ItemCollection Items { get; set; } = new ItemCollection();
        public ItemsPanelTemplate ItemsPanel { get; set; }
        public IEnumerable ItemsSource { get; set; } = new List<object>();
        public DataTemplate ItemTemplate { get; set; }
        public ItemContainerGenerator ItemContainerGenerator { get; set; }
        public String DisplayMemberPath { get; set; }
        public Style ItemContainerStyle { get; set; }
    }
}
