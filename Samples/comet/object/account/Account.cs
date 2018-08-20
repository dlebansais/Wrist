using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Account : IAccount, INotifyPropertyChanged
    {
        static Account()
        {
            Account NewAccount = new Account("a", SignInMethods.NameAndPassword, "b", "c");
            Accounts.Add(NewAccount);
        }

        public Account()
        {
            Email = null;
            SignInMethod = SignInMethods.None;
            Name = null;
            Password = null;
            KeepActiveIndex = -1;
        }

        public Account(string email, SignInMethods signInMethod, string name, string password)
        {
            Email = email;
            SignInMethod = signInMethod;
            Name = name;
            _Password = (signInMethod == SignInMethods.NameAndPassword ? password : null);
            KeepActiveIndex = -1;
        }

        public string Email { get; private set; }
        public SignInMethods SignInMethod { get; private set; }
        public string Name { get; private set; }

        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }
        private string _Password;

        public int KeepActiveIndex { get; private set; }
        public string FullName { get; set; }
        public string Location { get; set; }

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

        public static SignInError TryAddAccount(string email, SignInMethods method, string name, string password, out Account account)
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

        public static bool TrySignInAccount(string name, string password)
        {
            return TrySignInAccount(SignInMethods.None, name, password);
        }

        public static bool TrySignInAccount(SignInMethods signInMethod, string name, string password)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            foreach (Account Account in Accounts)
                if (Account.Name == name)
                {
                    if (signInMethod == SignInMethods.None || signInMethod == Account.SignInMethod)
                        if ((Account.SignInMethod == SignInMethods.NameOnly && string.IsNullOrEmpty(password)) || (Account.SignInMethod == SignInMethods.NameAndPassword && password == Account.Password))
                            return true;

                    break;
                }

            return false;
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
            Email = null;
            SignInMethod = SignInMethods.None;
            Name = null;
            Password = null;
            KeepActiveIndex = -1;
            FullName = null;
            Location = null;
            NotifyPropertyChanged(nameof(Email));
            NotifyPropertyChanged(nameof(SignInMethod));
            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(Password));
            NotifyPropertyChanged(nameof(KeepActiveIndex));
            NotifyPropertyChanged(nameof(FullName));
            NotifyPropertyChanged(nameof(Location));
        }

        public void On_Delete(string pageName, string sourceName, string sourceContent)
        {
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
