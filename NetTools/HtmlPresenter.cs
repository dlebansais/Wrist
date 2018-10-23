using Windows.UI.Xaml;

namespace NetTools
{
#if USE_RESTRICTED_FEATURES
    public class HtmlPresenter : CSHTML5.Native.Html.Controls.HtmlPresenter
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(HtmlPresenter), new PropertyMetadata(null));

        public string Content { get { return Html; } set { Html = value; } }
    }
#else
    public class HtmlPresenter : Windows.UI.Xaml.FrameworkElement
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(HtmlPresenter), new PropertyMetadata(null));

        public string Content { get; set; }
    }
#endif
}
