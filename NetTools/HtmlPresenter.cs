using Windows.UI.Xaml;

namespace NetTools
{
    public class HtmlPresenter : CSHTML5.Native.Html.Controls.HtmlPresenter
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(HtmlPresenter), new PropertyMetadata(null, OnContentChanged));

        public string Content
        {
            get { return Html; }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlPresenter Ctrl = (HtmlPresenter)d;
            string Content = (string)e.NewValue;

            Ctrl.Html = Content;
        }
    }
}
