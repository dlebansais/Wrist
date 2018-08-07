using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public partial class change_password_failed__5Page : Page
    {
        public change_password_failed__5Page()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Language Language { get { return App.Language; } }
        public Login Login { get { return App.Login; } }
        public NewsEntry NewsEntry { get { return App.NewsEntry; } }

        private void GoTo_Language_Switch__change_password_failed__5Page(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("change password failed #5", "other language", Content);
            (App.Current as App).GoTo("change password failed #5");
        }

        private void GoTo_homePage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("home");
        }

        private void GoTo_newsPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("news");
        }

        private void GoTo_loginPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("login");
        }

        private void GoTo_accountPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("account");
        }
    }
}
