using System;
using Windows.UI.Core;

namespace Windows.UI.Xaml
{
    public class DependencyObject : object
    {
        public CoreDispatcher Dispatcher { get; set; }
    }
}
