using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class NewsEntry : INewsEntry
    {
        public NewsEntry(LanguageStates languageState, string enu_title, string enu_text, string fra_title, string fra_text)
        {
            LanguageState = languageState;
            TitleTable.Add(LanguageStates.English, Language.ReplaceHtml(enu_title));
            TitleTable.Add(LanguageStates.French, Language.ReplaceHtml(fra_title));
            TextTable.Add(LanguageStates.English, Language.ReplaceHtml(enu_text));
            TextTable.Add(LanguageStates.French, Language.ReplaceHtml(fra_text));
        }

        public NewsEntry()
        {
        }

        public void SelectLanguage(LanguageStates languageState)
        {
            LanguageState = languageState;
        }

        public string Title { get { return TitleTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TitleTable { get; } = new Dictionary<LanguageStates, string>();
        public string Text { get { return TextTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TextTable { get; } = new Dictionary<LanguageStates, string>();
        private LanguageStates LanguageState;

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
