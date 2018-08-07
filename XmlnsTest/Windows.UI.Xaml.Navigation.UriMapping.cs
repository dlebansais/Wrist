using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Navigation
{
    public class UriMapping : DependencyObject
    {
        public Uri MappedUri { get; set; }
        public Uri Uri { get; set; }
    }
}
