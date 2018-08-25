using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace AppCSHtml5
{
    public class AccountManager : IAccountManager, INotifyPropertyChanged
    {
        public AccountManager()
        {
            Account NewAccount;

            NewAccount = new Account("a0", SignInMethods.NameAndPassword, "b", "c");
            Accounts.Add(NewAccount);

            NewAccount = new Account("a1", SignInMethods.NameOnly, "d", null);
            Accounts.Add(NewAccount);

            _ChangeMethodIndex = -1;
        }

        public IAccount SignedInAccount { get; private set; }

        public bool IsSignedIn { get { return SignedInAccount != null; } }
        public string Email { get { return SignedInAccount != null ? SignedInAccount.Email : null; } }
        public SignInMethods SignInMethod { get { return SignedInAccount != null ? SignedInAccount.SignInMethod : SignInMethods.None; } }
        public string Username { get { return SignedInAccount != null ? SignedInAccount.Username : null; } }
        public int KeepActiveIndex { get { return SignedInAccount != null ? SignedInAccount.KeepActiveIndex : -1; } }
        public string FullName { get { return SignedInAccount != null ? SignedInAccount.FullName : null; } }
        public string Location { get { return SignedInAccount != null ? SignedInAccount.Location : null; } }
        public bool Confirmed { get { return true; } set { } }
        public bool IsPasswordRequired { get { return SignInMethod == SignInMethods.NameAndPassword; } }
        public bool IsPasswordInvalidError { get; private set; }
        public int ChangeMethodIndex
        {
            get { return _ChangeMethodIndex; }
            set
            {
                if (value >= 0)
                    _ChangeMethodIndex = value;
            }
        }
        private int _ChangeMethodIndex;

        public string CurrentPassword
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(CurrentPassword));

                    IsPasswordInvalidError = false;
                    NotifyPropertyChanged(nameof(IsPasswordInvalidError));
                }
            }
        }
        private string _Password;

        public string NewPassword
        {
            get { return _NewPassword; }
            set
            {
                if (_NewPassword != value)
                {
                    _NewPassword = value;
                    NotifyPropertyChanged(nameof(NewPassword));
                }
            }
        }
        private string _NewPassword;

        public string NewUsername
        {
            get { return _NewUsername; }
            set
            {
                if (_NewUsername != value)
                {
                    _NewUsername = value;
                    NotifyPropertyChanged(nameof(NewUsername));
                }
            }
        }
        private string _NewUsername;

        public string NewEmail
        {
            get { return _NewEmail; }
            set
            {
                if (_NewEmail != value)
                {
                    _NewEmail = value;
                    NotifyPropertyChanged(nameof(NewEmail));
                }
            }
        }
        private string _NewEmail;

        public string NewFullName
        {
            get { return _NewFullName; }
            set
            {
                if (_NewFullName != value)
                {
                    _NewFullName = value;
                    NotifyPropertyChanged(nameof(NewFullName));
                    IsFullNameChanged = true;
                    NotifyPropertyChanged(nameof(IsFullNameChanged));
                }
            }
        }
        private string _NewFullName;

        public string NewLocation
        {
            get { return _NewLocation; }
            set
            {
                if (_NewLocation != value)
                {
                    _NewLocation = value;
                    NotifyPropertyChanged(nameof(NewLocation));
                    IsLocationChanged = true;
                    NotifyPropertyChanged(nameof(IsLocationChanged));
                }
            }
        }
        private string _NewLocation;

        public bool IsFullNameChanged { get; private set; }
        public bool IsLocationChanged { get; private set; }

        public SignInError TryAddAccount(string email, SignInMethods method, string username, string password, out Account account)
        {
            if (string.IsNullOrEmpty(email) || !(method == SignInMethods.NameOnly || method == SignInMethods.NameAndPassword) || string.IsNullOrEmpty(username) || (method == SignInMethods.NameAndPassword && string.IsNullOrEmpty(password)))
            {
                account = null;
                return SignInError.InternalError;
            }

            foreach (Account Account in Accounts)
                if (Account.Username == username)
                {
                    account = null;
                    return SignInError.NameAlreadyInUse;
                }

            account = new Account(email, method, username, password);
            Accounts.Add(account);

            return SignInError.None;
        }

        public bool TrySignInAccount(string name, string password)
        {
            return TrySignInAccount(SignInMethods.None, name, password);
        }

        public bool TrySignInAccount(SignInMethods signInMethod, string username, string password)
        {
            if (!string.IsNullOrEmpty(username))
            {
                foreach (Account Account in Accounts)
                    if (Account.Username == username)
                    {
                        if (signInMethod == SignInMethods.None || signInMethod == Account.SignInMethod)
                            if ((Account.SignInMethod == SignInMethods.NameOnly && string.IsNullOrEmpty(password)) || (Account.SignInMethod == SignInMethods.NameAndPassword && password == Account.Password))
                            {
                                SignedInAccount = Account;
                                _NewFullName = Account.FullName;
                                IsFullNameChanged = false;
                                _NewLocation = Account.Location;
                                IsLocationChanged = false;
                                IsPasswordInvalidError = false;
                                return true;
                            }

                        break;
                    }
            }

            return false;
        }

        public void On_ChangeEmail(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }
            else
                destinationPageName = "profile";
        }

        public void On_ChangePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }
            else
            {
                ((Account)SignedInAccount).ChangePassword(NewPassword);
                destinationPageName = "profile";
            }
        }

        public void On_AddPassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (SignedInAccount == null)
                destinationPageName = null;

            else
            {
                ((Account)SignedInAccount).AddPassword(NewPassword);

                _ChangeMethodIndex = -1;
                destinationPageName = "profile";
            }
        }

        public void On_RemovePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }
            else
            {
                ((Account)SignedInAccount).RemovePassword();

                _ChangeMethodIndex = -1;
                destinationPageName = "profile";
            }
        }

        public void On_CreateUsername(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (SignedInAccount == null)
                destinationPageName = null;

            else
            {
                ((Account)SignedInAccount).CreateUsername(Username);

                _ChangeMethodIndex = -1;
                destinationPageName = "profile";
            }
        }

        public void On_CreateUsernameAndPassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (SignedInAccount == null || string.IsNullOrEmpty(NewPassword))
                destinationPageName = null;

            else
            {
                ((Account)SignedInAccount).CreateUsernameAndPassword(Username, NewPassword);

                _ChangeMethodIndex = -1;
                destinationPageName = "profile";
            }
        }

        public void On_ChangeUsername(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (IsPasswordRequired && !ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }

            else if (string.IsNullOrEmpty(NewUsername) || SignedInAccount == null)
                destinationPageName = null;

            else
            {
                ((Account)SignedInAccount).ChangeUsername(NewUsername);
                destinationPageName = "profile";
            }
        }

        public void On_ChangeCertificate(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
        }

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

        public void On_SignOut(string pageName, string sourceName, string sourceContent)
        {
            SignedInAccount = null;
            _NewFullName = null;
            IsFullNameChanged = false;
            _NewLocation = null;
            IsLocationChanged = false;
            IsPasswordInvalidError = false;
            NotifyPropertyChanged(nameof(IsSignedIn));
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(SignInMethod));
            NotifyPropertyChanged(nameof(Username));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(NewFullName));
            NotifyPropertyChanged(nameof(IsFullNameChanged));
            NotifyPropertyChanged(nameof(NewLocation));
            NotifyPropertyChanged(nameof(IsLocationChanged));
            NotifyPropertyChanged(nameof(IsPasswordRequired));
            NotifyPropertyChanged(nameof(IsPasswordInvalidError));
        }

        public void On_ConfirmDelete(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (MessageBox.Show("This will delete your account, and cannot be recovered, are you sure?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                destinationPageName = "delete account";
            else
                destinationPageName = null;
        }

        public void On_SendDeleteEmail(string pageName, string sourceName, string sourceContent)
        {
            On_SignOut(pageName, sourceName, sourceContent);
            MessageBox.Show("Email sent");
        }

        public void OnPopupClosed_IsSignedIn()
        {
            NotifyPropertyChanged(nameof(IsSignedIn));
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(SignInMethod));
            NotifyPropertyChanged(nameof(IsPasswordRequired));
            NotifyPropertyChanged(nameof(Username));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(NewFullName));
            NotifyPropertyChanged(nameof(NewLocation));
        }

        private bool ClearPasswordAndCompare()
        {
            string TempPassword = CurrentPassword;
            CurrentPassword = null;

            return (SignedInAccount != null && ((Account)SignedInAccount).IsPasswordEqual(TempPassword));
        }

        private static List<Account> Accounts = new List<Account>();

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
