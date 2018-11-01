using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace AppCSHtml5
{
    public class AccountManager : ObjectBase, IAccountManager
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
                                IsPasswordInvalidError = false;
                                return true;
                            }

                        break;
                    }
            }

            return false;
        }

        public void On_ChangeEmail(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = PageNames.CurrentPage;
            }
            else
                destinationPageName = PageNames.profilePage;
        }

        public void On_ChangePassword(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = PageNames.CurrentPage;
            }
            else
            {
                ((Account)SignedInAccount).ChangePassword(NewPassword);
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_ChangeUsername(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (IsPasswordRequired && !ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = PageNames.CurrentPage;
            }

            else if (string.IsNullOrEmpty(NewUsername) || SignedInAccount == null)
                destinationPageName = PageNames.CurrentPage;

            else
            {
                ((Account)SignedInAccount).ChangeUsername(NewUsername);
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_SignOut(PageNames pageName, string sourceName, string sourceContent)
        {
            SignedInAccount = null;
            IsPasswordInvalidError = false;
            NotifyPropertyChanged(nameof(IsSignedIn));
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(SignInMethod));
            NotifyPropertyChanged(nameof(Username));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(IsPasswordRequired));
            NotifyPropertyChanged(nameof(IsPasswordInvalidError));
        }

        public void On_ConfirmDelete(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (MessageBox.Show("This will delete your account, and cannot be recovered, are you sure?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                destinationPageName = PageNames.delete_accountPage;
            else
                destinationPageName = PageNames.CurrentPage;
        }

        public void On_SendDeleteEmail(PageNames pageName, string sourceName, string sourceContent)
        {
            On_SignOut(pageName, sourceName, sourceContent);
            MessageBox.Show("Email sent");
        }

        public void On_AddPassword(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (SignedInAccount == null)
                destinationPageName = PageNames.CurrentPage;

            else
            {
                ((Account)SignedInAccount).AddPassword(NewPassword);

                _ChangeMethodIndex = -1;
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_RemovePassword(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (!ClearPasswordAndCompare())
            {
                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = PageNames.CurrentPage;
            }
            else
            {
                ((Account)SignedInAccount).RemovePassword();

                _ChangeMethodIndex = -1;
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_CreateUsername(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (SignedInAccount == null)
                destinationPageName = PageNames.CurrentPage;

            else
            {
                ((Account)SignedInAccount).CreateUsername(Username);

                _ChangeMethodIndex = -1;
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_CreateUsernameAndPassword(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            if (SignedInAccount == null || string.IsNullOrEmpty(NewPassword))
                destinationPageName = PageNames.CurrentPage;

            else
            {
                ((Account)SignedInAccount).CreateUsernameAndPassword(Username, NewPassword);

                _ChangeMethodIndex = -1;
                destinationPageName = PageNames.profilePage;
            }
        }

        public void On_ChangeCertificate(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            destinationPageName = PageNames.CurrentPage;
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
