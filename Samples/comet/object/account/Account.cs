using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Account : IAccount, INotifyPropertyChanged
    {
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
