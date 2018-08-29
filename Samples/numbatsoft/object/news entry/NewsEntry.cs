using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class NewsEntry : INewsEntry
    {
        public NewsEntry(LanguageStates languageState, string created, string enu_title, string enu_text, string fra_title, string fra_text)
        {
            LanguageState = languageState;
            DateTime ParsedCreated;
            DateTime.TryParse(created, out ParsedCreated);
            _Created = ParsedCreated;
            TitleTable.Add(LanguageStates.English, Language.ReplaceHtml(enu_title));
            TitleTable.Add(LanguageStates.French, Language.ReplaceHtml(fra_title));
            TextTable.Add(LanguageStates.English, Language.ReplaceHtml(enu_text));
            TextTable.Add(LanguageStates.French, Language.ReplaceHtml(fra_text));
        }

        public NewsEntry()
        {
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }

        public void SelectLanguage(LanguageStates languageState)
        {
            LanguageState = languageState;
        }

        public string Created
        {
            get
            {
                switch (LanguageState)
                {
                    default:
                    case LanguageStates.English:
                        return _Created.ToString("MMM d, yyyy", CultureInfo.GetCultureInfo("en-US"));

                    case LanguageStates.French:
                        return _Created.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"));
                }
            }
        }
        public DateTime _Created;

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
