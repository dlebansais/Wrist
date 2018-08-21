using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class SignIn : ISignIn, INotifyPropertyChanged
    {
        public SignIn()
        {
            KeepActiveIndex = -1;
            _SignInMethod = SignInMethods.None;
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged(nameof(Name));

                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                }
            }
        }
        private string _Name;

        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(Password));

                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                }
            }
        }
        private string _Password;

        public SignInMethods SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if ((value == SignInMethods.NameOnly || value == SignInMethods.NameAndPassword || value == SignInMethods.ThirdParty || value == SignInMethods.Certificate) && _SignInMethod != value)
                {
                    _SignInMethod = value;

                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                }
            }
        }
        private SignInMethods _SignInMethod;

        public int KeepActiveIndex { get; set; }
        public bool SignInError { get; set; }

        public void On_Cleanup(string pageName, string sourceName, string sourceContent)
        {
            SignInMethod = SignInMethods.None;
            Name = null;
            Password = null;
            SignInError = false;
            KeepActiveIndex = -1;
        }

        public void On_SignInNoMethod(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            string TempPassword = Password;
            Password = null;
            NotifyPropertyChanged(nameof(Password));

            if (AccountManager.TrySignInAccount(Name, TempPassword))
                CompleteSignIn(pageName, out destinationPageName);
            else
                FailSignIn(out destinationPageName);
        }

        public void On_SignInWithMethod1(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            On_SignInWithMethod(SignInMethods.NameOnly, pageName, out destinationPageName);
        }

        public void On_SignInWithMethod2(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            On_SignInWithMethod(SignInMethods.NameAndPassword, pageName, out destinationPageName);
        }

        private void On_SignInWithMethod(SignInMethods signInMethod, string pageName, out string destinationPageName)
        {
            string TempPassword = Password;
            Password = null;
            NotifyPropertyChanged(nameof(Password));

            if (AccountManager.TrySignInAccount(signInMethod, Name, TempPassword))
                CompleteSignIn(pageName, out destinationPageName);
            else
                FailSignIn(out destinationPageName);
        }

        private void CompleteSignIn(string pageName, out string destinationPageName)
        {
            if (pageName == "home" || pageName == "sign in" || pageName == "signed out")
                destinationPageName = "start";
            else
                destinationPageName = null;

            Name = null;
        }

        private void FailSignIn(out string destinationPageName)
        {
            SignInError = true;
            NotifyPropertyChanged(nameof(SignInError));

            destinationPageName = null;
        }

        public AccountManager AccountManager { get { return App.AccountManager; } }

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
