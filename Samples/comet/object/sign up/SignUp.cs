using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public enum SignInError
    {
        None,
        InternalError,
        NameAlreadyInUse,
    }

    public class SignUp : ObjectBase, ISignUp
    {
        public SignUp()
        {
            _SignInMethod = SignInMethods.None;
            KeepActiveIndex = -1;
        }

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

        public string Email
        {
            get { return _Email; }
            set
            {
                if (_Email != value)
                {
                    _Email = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
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

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
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

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
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

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private bool _Confirm3;

        public bool IsProfileReady { get { return Confirm1 && Confirm2 && Confirm3 && !string.IsNullOrEmpty(Email); } set { } }

        public SignInMethods SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if ((value == SignInMethods.NameOnly || value == SignInMethods.NameAndPassword || value == SignInMethods.ThirdParty) && _SignInMethod != value)
                {
                    _SignInMethod = value;

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private SignInMethods _SignInMethod;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged(nameof(Name));

                    NameError = false;
                    NotifyPropertyChanged(nameof(NameError));
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

                    SignUpError = false;
                    NotifyPropertyChanged(nameof(SignUpError));
                }
            }
        }
        private string _Password;

        public int KeepActiveIndex { get; set; }
        public bool NameError { get; set; }
        public bool SignUpError { get; set; }

        public void On_Cleanup(PageNames pageName, string sourceName, string sourceContent)
        {
            Name = null;
            Password = null;
            NameError = false;
            SignUpError = false;
            Email = null;
            Confirm1 = false;
            Confirm2 = false;
            Confirm3 = false;
            KeepActiveIndex = -1;
            SignInMethod = SignInMethods.None;
        }

        public void On_SignUp(PageNames pageName, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            Account NewAccount;
            SignInError Error = ((AccountManager)GetAccountManager).TryAddAccount(Email, SignInMethod, Name, Password, out NewAccount);

            switch (Error)
            {
                case SignInError.None:
                    break;

                default:
                case SignInError.NameAlreadyInUse:
                    NameError = true;
                    NotifyPropertyChanged(nameof(NameError));
                    SignUpError = true;
                    NotifyPropertyChanged(nameof(SignUpError));

                    destinationPageName = PageNames.CurrentPage;
                    return;
            }

            destinationPageName = PageNames.startPage;
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
