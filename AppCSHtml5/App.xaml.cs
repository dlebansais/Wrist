using Windows.UI.Xaml;

namespace AppCSHtml5
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Window.Current.Content = new homePage();
        }

        public static Language Language { get; } = new Language();
        public static Login Login { get; } = new Login();
        public static NewsEntry NewsEntry { get; } = new NewsEntry();

        public void GoTo(string pageName)
        {
            if (pageName == "account")
                Window.Current.Content = new accountPage();
            else if (pageName == "change email")
                Window.Current.Content = new change_emailPage();
            else if (pageName == "change password")
                Window.Current.Content = new change_passwordPage();
            else if (pageName == "change password failed #1")
                Window.Current.Content = new change_password_failed__1Page();
            else if (pageName == "change password failed #2")
                Window.Current.Content = new change_password_failed__2Page();
            else if (pageName == "change password failed #3")
                Window.Current.Content = new change_password_failed__3Page();
            else if (pageName == "change password failed #4")
                Window.Current.Content = new change_password_failed__4Page();
            else if (pageName == "change password failed #5")
                Window.Current.Content = new change_password_failed__5Page();
            else if (pageName == "change password success")
                Window.Current.Content = new change_password_successPage();
            else if (pageName == "change recovery")
                Window.Current.Content = new change_recoveryPage();
            else if (pageName == "home")
                Window.Current.Content = new homePage();
            else if (pageName == "login")
                Window.Current.Content = new loginPage();
            else if (pageName == "login failed")
                Window.Current.Content = new login_failedPage();
            else if (pageName == "news")
                Window.Current.Content = new newsPage();
        }
    }
}
