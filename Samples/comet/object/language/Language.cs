using DatabaseManager;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Language : ILanguage, INotifyPropertyChanged
    {
        public List<string> KeepActiveOptions
        {
            get
            {
                List<string> Result = new List<string>();

                Translation Translation = App.Translation;
                IDictionary<string, string> Strings = Translation.Strings;
                Result.Add(Strings["keep-active-option-1"]);
                Result.Add(Strings["keep-active-option-2"]);
                Result.Add(Strings["keep-active-option-3"]);

                return Result;
            }
        }

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
