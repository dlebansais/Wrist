using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class EqmlpReleaseNote : IEqmlpReleaseNote
    {
        public EqmlpReleaseNote(LanguageStates languageState, string revision, string enuText, string fraText)
        {
            LanguageState = languageState;

            string EnglishRevision = Language.ReplaceHtml(enuText);
            RevisionTable.Add(LanguageStates.English, EnglishRevision);

            string FrenchRevision = Language.ReplaceHtml(fraText);
            RevisionTable.Add(LanguageStates.French, FrenchRevision);
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }

        public string Revision { get { return RevisionTable[GetLanguage.LanguageState]; } }
        public Dictionary<LanguageStates, string> RevisionTable { get; } = new Dictionary<LanguageStates, string>();

        private LanguageStates LanguageState;

        public void SelectLanguage(LanguageStates languageState)
        {
            LanguageState = languageState;
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
