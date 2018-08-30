using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class NewsEntry : INewsEntry
    {
        public NewsEntry(LanguageStates languageState, string created, string enu_title, string enu_text, string fra_title, string fra_text)
        {
            LanguageState = languageState;

            ParseCreated(created);
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

        public string Created { get { return CreatedTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> CreatedTable { get; } = new Dictionary<LanguageStates, string>();
        public string Title { get { return TitleTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TitleTable { get; } = new Dictionary<LanguageStates, string>();
        public string Text { get { return TextTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TextTable { get; } = new Dictionary<LanguageStates, string>();
        private LanguageStates LanguageState;

        private void ParseCreated(string s)
        {
            int Year, Month, Day;
            int.TryParse(s.Substring(0, 4), out Year);
            int.TryParse(s.Substring(5, 2), out Month);
            int.TryParse(s.Substring(8, 2), out Day);
            if (Year >= 2000 && Year <= 5000 && Month >= 1 && Month <= 12 && Day >= 1 && Day <= 31)
            {
                List<string> MonthList = new List<string>() { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                CreatedTable.Add(LanguageStates.English, $"{MonthList[Month]} {Day}, {Year}");
                CreatedTable.Add(LanguageStates.French, $"{Day}/{Month}/{Year}");
            }
            else
            {
                CreatedTable.Add(LanguageStates.English, "");
                CreatedTable.Add(LanguageStates.French, "");
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
