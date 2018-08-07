using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Presentation
{
    public class Popup : Windows.UI.Xaml.Controls.Primitives.Popup
    {
        public Popup()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Border AsBorder = Child as Border;
            AsBorder.Loaded += OnChildLoaded;
        }

        private void OnChildLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ctrl = sender as FrameworkElement;
            if (ctrl != null)
                ctrl.DataContext = this.DataContext;
        }
    }
}
