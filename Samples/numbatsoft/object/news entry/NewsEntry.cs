using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            string EnglishTitle = Language.ReplaceHtml(enu_title);
            TitleTable.Add(LanguageStates.English, EnglishTitle);

            string FrenchTitle = Language.ReplaceHtml(fra_title);
            TitleTable.Add(LanguageStates.French, FrenchTitle);

            List<INewsEntryLink> LinkList;

            string EnglishText = Language.ReplaceHtml(enu_text);
            EnglishText = ReplaceLinks(EnglishText, out LinkList);
            TextTable.Add(LanguageStates.English, EnglishText);
            LinksTable.Add(LanguageStates.English, new ObservableCollection<INewsEntryLink>(LinkList));

            string FrenchText = Language.ReplaceHtml(fra_text);
            FrenchText = ReplaceLinks(FrenchText, out LinkList);
            TextTable.Add(LanguageStates.French, FrenchText);
            LinksTable.Add(LanguageStates.French, new ObservableCollection<INewsEntryLink>(LinkList));
        }

        public NewsEntry()
        {
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }

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
        public ObservableCollection<INewsEntryLink> Links { get { return LinksTable[LanguageState]; } }
        public Dictionary<LanguageStates, ObservableCollection<INewsEntryLink>> LinksTable { get; } = new Dictionary<LanguageStates, ObservableCollection<INewsEntryLink>>();

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

        private string ReplaceLinks(string text, out List<INewsEntryLink> linkList)
        {
            linkList = new List<INewsEntryLink>();

            int StartIndex = 0;
            string Pattern = "<a href=\"#root#";
            int ReferenceIndex = 1;

            while ((StartIndex = text.IndexOf(Pattern, StartIndex)) >= 0)
            {
                int EndIndex = text.IndexOf("\">", StartIndex + Pattern.Length);
                if (EndIndex > StartIndex + Pattern.Length)
                {
                    string PageLink = text.Substring(StartIndex + Pattern.Length, EndIndex - StartIndex - Pattern.Length);

                    int EndLinkIndex = text.IndexOf("</a>", EndIndex + 2);
                    if (EndLinkIndex > EndIndex + 2)
                    {
                        string LinkText = text.Substring(EndIndex + 2, EndLinkIndex - EndIndex - 2);

                        string Ref = $" [{ReferenceIndex}]";
                        text = text.Substring(0, StartIndex) + LinkText + Ref + text.Substring(EndLinkIndex + 4);

                        NewsEntryLink NewItem = new NewsEntryLink(ReferenceIndex, LinkText, PageLink);
                        linkList.Add(NewItem);

                        ReferenceIndex++;
                    }
                }
            }

            return text;
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
