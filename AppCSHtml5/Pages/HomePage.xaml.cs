using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public partial class homePage : Page
    {
        public homePage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Language Language { get { return App.Language; } }
        public Login Login { get { return App.Login; } }
        public NewsEntry NewsEntry { get { return App.NewsEntry; } }

        private void GoTo_Language_Switch__homePage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("home", "other language", Content);
            (App.Current as App).GoTo("home");
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

        private void GoTo_Language_Switch__newsPage(object sender, RoutedEventArgs e)
        {
            string Content = (sender as Button).Content as string;
            Language.On_Switch("home", "read more news", Content);
            (App.Current as App).GoTo("news");
        }
    }
}
