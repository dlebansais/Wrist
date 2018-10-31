using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class StatusEntry : ObjectBase, IStatusEntry
    {
        public StatusEntry(string id, string level, string label_enu, string detail_enu, string label_fra, string detail_fra)
        {
            int.TryParse(id, out int IdInt);
            Id = IdInt;

            int.TryParse(level, out int LevelInt);
            Level = LevelInt;

            string EnglishLabel = Language.ReplaceHtml(label_enu);
            LabelTable.Add(LanguageStates.English, EnglishLabel);

            string EnglishDetail = Language.ReplaceHtml(detail_enu);
            DetailTable.Add(LanguageStates.English, EnglishDetail);

            string FrenchLabel = Language.ReplaceHtml(label_fra);
            LabelTable.Add(LanguageStates.French, FrenchLabel);

            string FrenchDetail = Language.ReplaceHtml(detail_fra);
            DetailTable.Add(LanguageStates.French, FrenchDetail);
        }

        public int Id { get; private set; }
        public int Level { get; private set; }
        public string Label { get { return LabelTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> LabelTable { get; } = new Dictionary<LanguageStates, string>();
        public string Detail { get { return DetailTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> DetailTable { get; } = new Dictionary<LanguageStates, string>();
        public string FriendlyText { get { return Detail.Length > 0 ? $"{Label} ({Detail})" : Label; } }

        protected LanguageStates LanguageState { get { return GetLanguage.LanguageState; } }

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
