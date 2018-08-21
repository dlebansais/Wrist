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

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
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
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private string _Password;

        public string Email
        {
            get { return _Email; }
            set
            {
                if (_Email != value)
                {
                    _Email = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private string _Email;

        public bool Confirm1
        {
            get { return _Confirm1; }
            set
            {
                if (_Confirm1 != value)
                {
                    _Confirm1 = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private bool _Confirm1;

        public bool Confirm2
        {
            get { return _Confirm2; }
            set
            {
                if (_Confirm2 != value)
                {
                    _Confirm2 = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private bool _Confirm2;

        public bool Confirm3
        {
            get { return _Confirm3; }
            set
            {
                if (_Confirm3 != value)
                {
                    _Confirm3 = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private bool _Confirm3;

        public SignInMethods SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if ((value == SignInMethods.NameOnly || value == SignInMethods.NameAndPassword || value == SignInMethods.ThirdParty || value == SignInMethods.Certificate) && _SignInMethod != value)
                {
                    _SignInMethod = value;

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private SignInMethods _SignInMethod;

        public bool IsReady
        {
            get { return _IsReady; }
            set
            {
                if (value == true)
                    _IsReady = true;
            }
        }
        private bool _IsReady;

        public string NewPassword { get; set; }
        public int KeepActiveIndex { get; set; }
        public bool UserNameError { get; set; }
        public bool SignUpNameError { get; set; }
        public bool SignInError { get; set; }
        public bool SignUpError { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }

        public bool IsProfileReady { get { return Confirm1 && Confirm2 && Confirm3 && !string.IsNullOrEmpty(Email); } set { } }

        public void On_Cleanup(string pageName, string sourceName, string sourceContent)
        {
            Name = null;
            Password = null;
            NewPassword = null;
            UserNameError = false;
            SignUpNameError = false;
            SignInError = false;
            SignUpError = false;
            Email = null;
            Confirm1 = false;
            Confirm2 = false;
            Confirm3 = false;
            KeepActiveIndex = -1;
            FullName = null;
            Location = null;
            SignInMethod = SignInMethods.None;
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
            if (pageName == "home" || pageName == "sign up" || pageName == "signed out")
                destinationPageName = "start";
            else
                destinationPageName = null;
        }

        private void FailSignIn(out string destinationPageName)
        {
            UserNameError = true;
            NotifyPropertyChanged(nameof(UserNameError));
            SignInError = true;
            NotifyPropertyChanged(nameof(SignInError));

            destinationPageName = null;
        }

        public void On_SignOut(string pageName, string sourceName, string sourceContent)
        {
            Name = null;
            Email = null;
            Confirm1 = false;
            Confirm2 = false;
            Confirm3 = false;
            KeepActiveIndex = -1;
            FullName = null;
            Location = null;
            SignInMethod = SignInMethods.None;
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(Confirm1));
            NotifyPropertyChanged(nameof(Confirm2));
            NotifyPropertyChanged(nameof(Confirm3));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(SignInMethod));
        }

        public void On_Disconnect(string pageName, string sourceName, string sourceContent)
        {
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
