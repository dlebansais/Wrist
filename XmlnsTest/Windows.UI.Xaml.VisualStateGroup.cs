using System.Windows.Markup;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Windows.UI.Xaml
{
    [ContentProperty("States")]
    public class VisualStateGroup : DependencyObject
    {
        public VisualState CurrentState { get; set; }
        public String Name { get; set; }
        public IList States { get; set; } = new List<object>();
    }
}
