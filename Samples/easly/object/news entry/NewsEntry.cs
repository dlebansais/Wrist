using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class NewsEntry : ObjectBase, INewsEntry
    {
        public NewsEntry(string created, string enu_title, string enu_text, string fra_title, string fra_text)
        {
            string EnuDate, FraDate;
            ParseCreated(created, out EnuDate, out FraDate);
            CreatedTable.Add(LanguageStates.English, EnuDate);
            CreatedTable.Add(LanguageStates.French, FraDate);

            string EnglishTitle = Language.ReplaceHtml(enu_title);
            TitleTable.Add(LanguageStates.English, EnglishTitle);

            string FrenchTitle = Language.ReplaceHtml(fra_title);
            TitleTable.Add(LanguageStates.French, FrenchTitle);

            string EnglishText = Language.ReplaceHtml(enu_text);
            TextTable.Add(LanguageStates.English, EnglishText);

            string FrenchText = Language.ReplaceHtml(fra_text);
            TextTable.Add(LanguageStates.French, FrenchText);
        }

        public string Created { get { return CreatedTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> CreatedTable { get; } = new Dictionary<LanguageStates, string>();
        public string Title { get { return TitleTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TitleTable { get; } = new Dictionary<LanguageStates, string>();
        public string Text { get { return TextTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> TextTable { get; } = new Dictionary<LanguageStates, string>();

        protected LanguageStates LanguageState { get { return GetLanguage.LanguageState; } }

        public static void ParseCreated(string s, out string enuDate, out string fraDate)
        {
            int Year, Month, Day;
            int.TryParse(s.Substring(0, 4), out Year);
            int.TryParse(s.Substring(5, 2), out Month);
            int.TryParse(s.Substring(8, 2), out Day);
            if (Year >= 2000 && Year <= 5000 && Month >= 1 && Month <= 12 && Day >= 1 && Day <= 31)
            {
                List<string> MonthList = new List<string>() { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                enuDate = $"{MonthList[Month]} {Day}, {Year}";
                fraDate = $"{Day}/{Month}/{Year}";
            }
            else
            {
                enuDate = "";
                fraDate = "";
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
