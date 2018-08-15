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
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string NewPassword { get; set; }
        public bool IsSignedIn { get; set; }

        public string Email
        {
            get { return _Email; }
            set
            {
                if (_Email != value)
                {
                    _Email = value;
                    NotifyPropertyChanged(nameof(IsProfileReady));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));
                }
            }
        }
        private bool _Confirm3;

        public int KeepActiveIndex { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }

        public int SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if (value > 0 && value < 4 && _SignInMethod != value)
                {
                    _SignInMethod = value;
                    NotifyPropertyChanged(nameof(IsSignInMethod1));
                    NotifyPropertyChanged(nameof(IsSignInMethod2));
                    NotifyPropertyChanged(nameof(IsSignInMethod3));
                    NotifyPropertyChanged(nameof(IsSignInMethod4));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));
                }
            }
        }
        public bool IsSignInMethod1 { get { return SignInMethod == 1; } set { } }
        public bool IsSignInMethod2 { get { return SignInMethod == 2; } set { } }
        public bool IsSignInMethod3 { get { return SignInMethod == 3; } set { } }
        public bool IsSignInMethod4 { get { return SignInMethod == 4; } set { } }
        private int _SignInMethod;

        public bool Ready
        {
            get { return _Ready; }
            set
            {
                if (_Ready != value)
                {
                    _Ready = value;
                    NotifyPropertyChanged(nameof(Ready));
                }
            }
        }
        private bool _Ready;

        public bool IsProfileReady { get { return Confirm1 && Confirm2 && Confirm3 && !string.IsNullOrEmpty(Email); } set { } }

        public bool IsSignInMethod1Possible { get { return SignInMethod == 1 && IsProfileReady && !string.IsNullOrEmpty(Name); } set { } }
        public bool IsSignInMethod2Possible { get { return SignInMethod == 2 && IsProfileReady && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword); } set { } }

        public void On_SignIn(string pageName, string sourceName, string sourceContent)
        {
            IsSignedIn = true;
            Password = null;
            ConfirmPassword = null;
            NotifyPropertyChanged(nameof(IsSignedIn));
            NotifyPropertyChanged(nameof(Password));
            NotifyPropertyChanged(nameof(ConfirmPassword));
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
            Name = null;
            IsSignedIn = false;
            Email = null;
            Confirm1 = false;
            Confirm2 = false;
            Confirm3 = false;
            KeepActiveIndex = 0;
            FullName = null;
            Location = null;
            SignInMethod = 0;
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(IsSignedIn));
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
