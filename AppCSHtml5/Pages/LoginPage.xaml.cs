using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public partial class loginPage : Page
    {
        public loginPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Language Language { get { return App.Language; } }
        public Login Login { get { return App.Login; } }
        public NewsEntry NewsEntry { get { return App.NewsEntry; } }

        private void GoTo_Language_Switch__loginPage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("login", "other language", Content);
            (App.Current as App).GoTo("login");
        }

        private void GoTo_homePage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("home");
        }

        private void GoTo_newsPage(object sender, RoutedEventArgs e)
        {
            (App.Current as App).GoTo("news");
        }

        private void GoTo_Login_Login__Any(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            string DestinationPageName;
            Login.On_Login("login", "login", Content, out DestinationPageName);
            (App.Current as App).GoTo(DestinationPageName);
        }
    }
}
