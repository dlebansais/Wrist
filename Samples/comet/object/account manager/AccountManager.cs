﻿using System.Collections.Generic;
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
            Account NewAccount = new Account("a", SignInMethods.NameAndPassword, "b", "c");
            Accounts.Add(NewAccount);
        }

        public Account SignedInAccount { get; private set; }

        public bool IsSignedIn { get { return SignedInAccount != null; } }
        public string Email { get { return SignedInAccount != null ? SignedInAccount.Email : null; } }
        public SignInMethods SignInMethod { get { return SignedInAccount != null ? SignedInAccount.SignInMethod : SignInMethods.None; } }
        public string Name { get { return SignedInAccount != null ? SignedInAccount.Name : null; } }
        public int KeepActiveIndex { get { return SignedInAccount != null ? SignedInAccount.KeepActiveIndex : -1; } }
        public string FullName { get { return SignedInAccount != null ? SignedInAccount.FullName : null; } }
        public string Location { get { return SignedInAccount != null ? SignedInAccount.Location : null; } }
        public bool Confirmed { get { return true; } set { } }
        public bool IsPasswordInvalidError { get; private set; }

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

        public SignInError TryAddAccount(string email, SignInMethods method, string name, string password, out Account account)
        {
            if (string.IsNullOrEmpty(email) || !(method == SignInMethods.NameOnly || method == SignInMethods.NameAndPassword) || string.IsNullOrEmpty(name) || (method == SignInMethods.NameAndPassword && string.IsNullOrEmpty(password)))
            {
                account = null;
                return SignInError.InternalError;
            }

            foreach (Account Account in Accounts)
                if (Account.Name == name)
                {
                    account = null;
                    return SignInError.NameAlreadyInUse;
                }

            account = new Account(email, method, name, password);
            Accounts.Add(account);

            return SignInError.None;
        }

        public bool TrySignInAccount(string name, string password)
        {
            return TrySignInAccount(SignInMethods.None, name, password);
        }

        public bool TrySignInAccount(SignInMethods signInMethod, string name, string password)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foreach (Account Account in Accounts)
                    if (Account.Name == name)
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
            if (SignedInAccount == null || string.IsNullOrEmpty(SignedInAccount.Password) || CurrentPassword != SignedInAccount.Password)
            {
                CurrentPassword = null;

                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }
            else
                destinationPageName = "profile";
        }

        public void On_ChangePassword(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            if (SignedInAccount == null || string.IsNullOrEmpty(SignedInAccount.Password) || CurrentPassword != SignedInAccount.Password)
            {
                CurrentPassword = null;

                IsPasswordInvalidError = true;
                NotifyPropertyChanged(nameof(IsPasswordInvalidError));

                destinationPageName = null;
            }
            else
                destinationPageName = "profile";
        }

        public void On_ChangeMethod(string pageName, string sourceName, string sourceContent, out string destinationPageName)
        {
            destinationPageName = null;
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
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(NewFullName));
            NotifyPropertyChanged(nameof(IsFullNameChanged));
            NotifyPropertyChanged(nameof(NewLocation));
            NotifyPropertyChanged(nameof(IsLocationChanged));
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
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
            NotifyPropertyChanged(nameof(NewFullName));
            NotifyPropertyChanged(nameof(NewLocation));
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
