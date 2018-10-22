using Windows.UI.Xaml;

namespace NetTools
{
#if USE_RESTRICTED_FEATURES
    public class HtmlPresenter : CSHTML5.Native.Html.Controls.HtmlPresenter
    {
    }
#else
    public class HtmlPresenter : Windows.UI.Xaml.FrameworkElement
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(HtmlPresenter), new PropertyMetadata(null));

        public string Html { get; set; }
    }
#endif
}
