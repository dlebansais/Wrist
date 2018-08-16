using DatabaseManager;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Login : ILogin, INotifyPropertyChanged
    {
        public Login()
        {
            KeepActiveIndex = 1;
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        private string _Password;

        public string ConfirmPassword
        {
            get { return _ConfirmPassword; }
            set
            {
                if (_ConfirmPassword != value)
                {
                    _ConfirmPassword = value;
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        private string _ConfirmPassword;

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
                    NotifyPropertyChanged(nameof(IsSignInMethod1));
                    NotifyPropertyChanged(nameof(IsSignInMethod2));
                    NotifyPropertyChanged(nameof(IsSignInMethod3));
                    NotifyPropertyChanged(nameof(IsSignInMethod4));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1));
                    NotifyPropertyChanged(nameof(IsSignInMethod2));
                    NotifyPropertyChanged(nameof(IsSignInMethod3));
                    NotifyPropertyChanged(nameof(IsSignInMethod4));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1));
                    NotifyPropertyChanged(nameof(IsSignInMethod2));
                    NotifyPropertyChanged(nameof(IsSignInMethod3));
                    NotifyPropertyChanged(nameof(IsSignInMethod4));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
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
                    NotifyPropertyChanged(nameof(IsSignInMethod1));
                    NotifyPropertyChanged(nameof(IsSignInMethod2));
                    NotifyPropertyChanged(nameof(IsSignInMethod3));
                    NotifyPropertyChanged(nameof(IsSignInMethod4));
                    NotifyPropertyChanged(nameof(IsSignInMethod1Possible));
                    NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        private bool _Confirm3;

        public int KeepActiveIndex
        {
            get { return _KeepActiveIndex; }
            set
            {
                if (_KeepActiveIndex != value)
                {
                    _KeepActiveIndex = value;
                    NotifyPropertyChanged(nameof(KeepActiveIndex));
                }
            }
        }
        private int _KeepActiveIndex;

        public bool SignUpNameError { get; set; }
        public bool SignUpConfirmPasswordError { get; set; }

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

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        public bool IsSignInMethod1 { get { return SignInMethod == 1 && IsProfileReady; } set { } }
        public bool IsSignInMethod2 { get { return SignInMethod == 2 && IsProfileReady; } set { } }
        public bool IsSignInMethod3 { get { return SignInMethod == 3 && IsProfileReady; } set { } }
        public bool IsSignInMethod4 { get { return SignInMethod == 4 && IsProfileReady; } set { } }
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

        public bool IsSignInMethod1Possible { get { return IsSignInMethod1 && !string.IsNullOrEmpty(Name) && !SignUpNameError; } set { } }
        public bool IsSignInMethod2Possible { get { return IsSignInMethod2 && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && !SignUpConfirmPasswordError; } set { } }

        public void On_SignUp(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            Account NewAccount = new Account();
            NewAccount.SignInMethod = SignInMethod;
            NewAccount.KeepActiveIndex = KeepActiveIndex;

            switch (SignInMethod)
            {
                case 1:
                    for (int i = 0; i < Accounts.Count; i++)
                        if (Accounts[i].Name == Name)
                        {
                            SignUpNameError = true;
                            NotifyPropertyChanged(nameof(SignUpNameError));
                            NotifyPropertyChanged(nameof(IsSignInMethod1Possible));

                            destinationPageName = null;
                            return;
                        }

                    NewAccount.Name = Name;
                    NewAccount.Password = null;
                    Accounts.Add(NewAccount);
                    break;

                case 2:
                    if (Password != ConfirmPassword)
                    {
                        SignUpConfirmPasswordError = true;
                        NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                        NotifyPropertyChanged(nameof(IsSignInMethod2Possible));

                        destinationPageName = null;
                        return;
                    }

                    for (int i = 0; i < Accounts.Count; i++)
                        if (Accounts[i].Name == Name)
                        {
                            SignUpNameError = true;
                            NotifyPropertyChanged(nameof(SignUpNameError));

                            destinationPageName = null;
                            return;
                        }

                    NewAccount.Name = Name;
                    NewAccount.Password = Password;
                    Accounts.Add(NewAccount);
                    break;

                default:
                    destinationPageName = null;
                    return;
            }

            destinationPageName = "start";
        }

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

        private List<Account> Accounts = new List<Account>();

        public class Account
        {
            public int SignInMethod;
            public string Name;
            public string Password;
            public int KeepActiveIndex;
        };

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
