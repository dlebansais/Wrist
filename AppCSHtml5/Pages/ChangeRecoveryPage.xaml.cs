using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public partial class change_recoveryPage : Page
    {
        public change_recoveryPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Language Language { get { return App.Language; } }
        public Login Login { get { return App.Login; } }
        public NewsEntry NewsEntry { get { return App.NewsEntry; } }

        private void GoTo_Language_Switch__change_recoveryPage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("change recovery", "other language", Content);
            (App.Current as App).GoTo("change recovery");
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
            Login.On_Logout("change recovery", "logout", Content);
            (App.Current as App).GoTo("home");
        }
    }
}
