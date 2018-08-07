using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public partial class accountPage : Page
    {
        public accountPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Language Language { get { return App.Language; } }
        public Login Login { get { return App.Login; } }
        public NewsEntry NewsEntry { get { return App.NewsEntry; } }

        private void GoTo_Language_Switch__accountPage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("account", "other language", Content);
            (App.Current as App).GoTo("account");
        }

        private void GoTo_homePage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("home");
        }

        private void GoTo_newsPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("news");
        }

        private void GoTo_Login_Logout__homePage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Login.On_Logout("account", "logout", Content);
            (App.Current as App).GoTo("home");
        }

        private void GoTo_change_passwordPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("change password");
        }

        private void GoTo_change_emailPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("change email");
        }

        private void GoTo_change_recoveryPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("change recovery");
        }
    }
}
