using System.Windows.Markup;
using System;
using Windows.UI.Xaml.Navigation;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Content")]
    public class Frame : ContentControl
    {
        public Boolean CanGoBack { get; set; }
        public Boolean CanGoForward { get; set; }
        public Uri Source { get; set; }
        public JournalOwnership JournalOwnership { get; set; }
        public UriMapperBase UriMapper { get; set; }
    }
}
