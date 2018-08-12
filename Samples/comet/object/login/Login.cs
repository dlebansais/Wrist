using DatabaseManager;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Login : ILogin, INotifyPropertyChanged
    {
        public Login()
        {
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public int SignInMethod { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Confirm1 { get; set; }
        public bool Confirm2 { get; set; }
        public bool Confirm3 { get; set; }
        public int KeepActiveIndex { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }

        public void On_SignIn(string pageName, string sourceName, string sourceContent)
        {

        }

        public void On_ChangeEmail(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

        public void On_ChangePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

        public void On_ChangeCertificate(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

        public void On_SignOut(string pageName, string sourceName, string sourceContent)
        {
        }

        public void On_Disconnect(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

        public void On_Delete(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

        private Database Database = Database.Current;

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
