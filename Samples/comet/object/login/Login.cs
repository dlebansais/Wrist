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
            KeepActiveIndex = -1;
            _SignInMethod = -1;

            Account NewAccount = new Account();
            NewAccount.SignInMethod = 0;
            NewAccount.KeepActiveIndex = 1;
            NewAccount.Name = "a";
            NewAccount.Password = "b";
            Accounts.Add(NewAccount);
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

                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
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

                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
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

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
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

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
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

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
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

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
                }
            }
        }
        private bool _Confirm3;

        public int KeepActiveIndex { get; set; }

        public bool SignInError { get; set; }
        public bool UserNameError { get; set; }
        public bool ConfirmPasswordError { get; set; }

        public string FullName { get; set; }
        public string Location { get; set; }

        public int SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if (value >= 0 && value <= 3 && _SignInMethod != value)
                {
                    _SignInMethod = value;

                    UserNameError = false;
                    NotifyPropertyChanged(nameof(UserNameError));
                    ConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(ConfirmPasswordError));
                    SignInError = false;
                    NotifyPropertyChanged(nameof(SignInError));
                }
            }
        }
        private int _SignInMethod;

        public bool IsReady { get; set; }

        public bool IsProfileReady { get { return Confirm1 && Confirm2 && Confirm3 && !string.IsNullOrEmpty(Email); } set { } }

        public void On_Cleanup(string pageName, string sourceName, string sourceContent)
        {
            Name = null;
            Password = null;
            ConfirmPassword = null;
            NewPassword = null;
            SignInError = false;
            UserNameError = false;
            ConfirmPasswordError = false;
            IsSignedIn = false;
            Email = null;
            Confirm1 = false;
            Confirm2 = false;
            Confirm3 = false;
            KeepActiveIndex = -1;
            FullName = null;
            Location = null;
            SignInMethod = -1;
        }

        public void On_SignUp(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            Account NewAccount = new Account();
            NewAccount.SignInMethod = SignInMethod;
            NewAccount.KeepActiveIndex = KeepActiveIndex;

            switch (SignInMethod)
            {
                case 0:
                    for (int i = 0; i < Accounts.Count; i++)
                        if (Accounts[i].Name == Name)
                        {
                            UserNameError = true;
                            NotifyPropertyChanged(nameof(UserNameError));

                            destinationPageName = null;
                            return;
                        }

                    NewAccount.Name = Name;
                    NewAccount.Password = null;
                    Accounts.Add(NewAccount);
                    break;

                case 1:
                    if (Password != ConfirmPassword)
                    {
                        ConfirmPasswordError = true;
                        NotifyPropertyChanged(nameof(ConfirmPasswordError));

                        destinationPageName = null;
                        return;
                    }

                    for (int i = 0; i < Accounts.Count; i++)
                        if (Accounts[i].Name == Name)
                        {
                            UserNameError = true;
                            NotifyPropertyChanged(nameof(UserNameError));

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

        private bool IsLoginValidMethod0()
        {
            foreach (Account Account in Accounts)
                if (Account.Name == Name)
                    return (Account.SignInMethod == 0);

            return false;
        }

        private bool IsLoginValidMethod1(string testPassword)
        {
            foreach (Account Account in Accounts)
                if (Account.Name == Name)
                    return (Account.SignInMethod == 1 && Account.Password == testPassword);

            return false;
        }

        public void On_SignIn(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            string TestPassword = Password;
            Password = null;
            NotifyPropertyChanged(nameof(Password));

            bool IsSignInValid = ((SignInMethod == 0 && IsLoginValidMethod0()) || (SignInMethod == 1 && IsLoginValidMethod1(TestPassword)));

            if (!IsSignInValid)
            {
                UserNameError = true;
                NotifyPropertyChanged(nameof(UserNameError));
                SignInError = true;
                NotifyPropertyChanged(nameof(SignInError));

                destinationPageName = null;
                return;
            }

            IsSignedIn = true;
            NotifyPropertyChanged(nameof(IsSignedIn));

            if (pageName == "home")
                destinationPageName = "start";
            else
                destinationPageName = null;
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
            KeepActiveIndex = -1;
            FullName = null;
            Location = null;
            SignInMethod = -1;
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
