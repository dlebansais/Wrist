using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Account : IAccount
    {
        public Account(string email, SignInMethods signInMethod, string username, string password)
        {
            Email = email;
            SignInMethod = signInMethod;
            Username = username;
            Password = (signInMethod == SignInMethods.NameAndPassword ? password : null);
            KeepActiveIndex = -1;
        }

        public Translation GetTranslation { get { return App.GetTranslation; } }
        public IAccountManager GetAccountManager { get { return App.GetAccountManager; } }
        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ISignIn GetSignIn { get { return App.GetSignIn; } }
        public ISignUp GetSignUp { get { return App.GetSignUp; } }

        public string Email { get; private set; }
        public SignInMethods SignInMethod { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int KeepActiveIndex { get; set; }

        public string FullName
        {
            get { return _FullName; }
            set
            {
                if (_FullName != value)
                {
                    _FullName = value;
                    NotifyPropertyChanged(nameof(FullName));
                    IsFullNameChanged = true;
                    NotifyPropertyChanged(nameof(IsFullNameChanged));
                }
            }
        }
        public bool IsFullNameChanged { get; private set; }
        private string _FullName;

        public string Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    NotifyPropertyChanged(nameof(Location));
                    IsLocationChanged = true;
                    NotifyPropertyChanged(nameof(IsLocationChanged));
                }
            }
        }
        public bool IsLocationChanged { get; private set; }
        private string _Location;

        public void On_UpdateFullName(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            IsFullNameChanged = false;
            NotifyPropertyChanged(nameof(IsFullNameChanged));

            destinationPageName = null;
        }

        public void On_UpdateLocation(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            IsLocationChanged = false;
            NotifyPropertyChanged(nameof(IsLocationChanged));

            destinationPageName = null;
        }

        public bool IsPasswordEqual(string password)
        {
            return !string.IsNullOrEmpty(password) && (password == Password);
        }

        public void ChangeUsername(string username)
        {
            Username = username;
        }

        public void ChangePassword(string password)
        {
            Password = password;
        }

        public void AddPassword(string password)
        {
            SignInMethod = SignInMethods.NameAndPassword;
            Password = password;
        }

        public void RemovePassword()
        {
            SignInMethod = SignInMethods.NameOnly;
            Password = null;
        }

        public void CreateUsername(string username)
        {
            SignInMethod = SignInMethods.NameOnly;
            Username = username;
            Password = null;
        }

        public void CreateUsernameAndPassword(string username, string password)
        {
            SignInMethod = SignInMethods.NameAndPassword;
            Username = username;
            Password = password;
        }

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
