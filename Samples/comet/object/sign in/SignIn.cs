using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class SignIn : ISignIn
    {
        public SignIn()
        {
            KeepActiveIndex = -1;
            _SignInMethod = SignInMethods.None;
        }

        public Translation GetTranslation { get { return App.GetTranslation; } }
        public IAccountManager GetAccountManager { get { return App.GetAccountManager; } }
        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ISignIn GetSignIn { get { return App.GetSignIn; } }
        public ISignUp GetSignUp { get { return App.GetSignUp; } }

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

        public void On_Cleanup(PageNames pageName, string sourceName, string sourceContent)
        {
            SignInMethod = SignInMethods.None;
            Name = null;
            Password = null;
            SignInError = false;
            KeepActiveIndex = -1;
        }

        public void On_SignInNoMethod(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            string TempPassword = Password;
            Password = null;
            NotifyPropertyChanged(nameof(Password));

            if (((AccountManager)GetAccountManager).TrySignInAccount(Name, TempPassword))
                CompleteSignIn(pageName, out destinationPageName);
            else
                FailSignIn(out destinationPageName);
        }

        public void On_SignInWithMethod1(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            On_SignInWithMethod(SignInMethods.NameOnly, pageName, out destinationPageName);
        }

        public void On_SignInWithMethod2(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            On_SignInWithMethod(SignInMethods.NameAndPassword, pageName, out destinationPageName);
        }

        private void On_SignInWithMethod(SignInMethods signInMethod, PageNames pageName, out PageNames destinationPageName)
        {
            string TempPassword = Password;
            Password = null;
            NotifyPropertyChanged(nameof(Password));

            if (((AccountManager)GetAccountManager).TrySignInAccount(signInMethod, Name, TempPassword))
                CompleteSignIn(pageName, out destinationPageName);
            else
                FailSignIn(out destinationPageName);
        }

        private void CompleteSignIn(PageNames pageName, out PageNames destinationPageName)
        {
            if (pageName == PageNames.homePage || pageName == PageNames.sign_inPage || pageName == PageNames.signed_outPage)
                destinationPageName = PageNames.startPage;
            else
                destinationPageName = PageNames.CurrentPage;

            Name = null;
        }

        private void FailSignIn(out PageNames destinationPageName)
        {
            SignInError = true;
            NotifyPropertyChanged(nameof(SignInError));

            destinationPageName = PageNames.CurrentPage;
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
