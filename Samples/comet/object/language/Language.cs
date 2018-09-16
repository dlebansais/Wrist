using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Language : ObjectBase, ILanguage
    {
        public ObservableCollection<string> KeepActiveOptions
        {
            get
            {
                if (_KeepActiveOptions == null)
                {
                    _KeepActiveOptions = new ObservableCollection<string>();

                    IDictionary<string, string> Strings = GetTranslation.Strings;
                    _KeepActiveOptions.Add(Strings["*keep-active-option-1"]);
                    _KeepActiveOptions.Add(Strings["*keep-active-option-2"]);
                    _KeepActiveOptions.Add(Strings["*keep-active-option-3"]);
                }

                return _KeepActiveOptions;
            }
        }
        private ObservableCollection<string> _KeepActiveOptions;

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
