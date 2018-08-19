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
            KeepActiveIndex1 = 1;
            KeepActiveIndex2 = 1;

            Account NewAccount = new Account();
            NewAccount.SignInMethod = 1;
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

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
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

                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
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

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        private bool _Confirm3;

        public int KeepActiveIndex1 { get; set; }
        public int KeepActiveIndex2 { get; set; }
        public int KeepActiveIndex
        {
            get
            {
                switch (SignInMethod)
                {
                    case 1:
                        return KeepActiveIndex1;
                    case 2:
                        return KeepActiveIndex2;

                    default:
                        return -1;
                }
            }
        }

        public bool SignInError { get; set; }
        public bool SignUpNameError { get; set; }
        public bool SignUpConfirmPasswordError { get; set; }

        public string FullName { get; set; }
        public string Location { get; set; }

        public int SignInMethod
        {
            get { return _SignInMethod; }
            set
            {
                if (value >= 1 && value <= 4 && _SignInMethod != value)
                {
                    _SignInMethod = value;

                    switch (value)
                    {
                        case 1:
                            KeepActiveIndex1 = KeepActiveIndex2;
                            NotifyPropertyChanged(nameof(KeepActiveIndex1));
                            break;
                        case 2:
                            KeepActiveIndex2 = KeepActiveIndex1;
                            NotifyPropertyChanged(nameof(KeepActiveIndex2));
                            break;
                    }

                    SignUpNameError = false;
                    NotifyPropertyChanged(nameof(SignUpNameError));
                    SignUpConfirmPasswordError = false;
                    NotifyPropertyChanged(nameof(SignUpConfirmPasswordError));
                }
            }
        }
        private int _SignInMethod;

        public bool IsReady { get; set; }

        public bool IsProfileReady { get { return Confirm1 && Confirm2 && Confirm3 && !string.IsNullOrEmpty(Email); } set { } }

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

        public void On_SignIn(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            int i;
            for (i = 0; i < Accounts.Count; i++)
            {
                Account Account = Accounts[i];
                if (Account.Name == Name && Account.Password == Password)
                    break;
            }
            if (i >= Accounts.Count)
            {
                Password = null;
                NotifyPropertyChanged(nameof(Password));

                SignInError = true;
                NotifyPropertyChanged(nameof(SignInError));

                destinationPageName = null;
                return;
            }

            IsSignedIn = true;
            Password = null;
            NotifyPropertyChanged(nameof(IsSignedIn));
            NotifyPropertyChanged(nameof(Password));

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
            KeepActiveIndex1 = -1;
            KeepActiveIndex2 = -1;
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
