using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class FeatureEntry : ObjectBase, IFeatureEntry
    {
        public FeatureEntry(string id, string status_id, string name_enu, string status_enu, string comments_enu, string name_fra, string status_fra, string comments_fra)
        {
            int.TryParse(id, out int IdInt);
            Id = IdInt;

            int.TryParse(status_id, out int StatusInt);
            StatusId = StatusInt;

            string EnglishName = Language.ReplaceHtml(name_enu);
            NameTable.Add(LanguageStates.English, EnglishName);

            string EnglishStatus = Language.ReplaceHtml(status_enu);
            StatusTable.Add(LanguageStates.English, EnglishStatus);

            string EnglishComments = Language.ReplaceHtml(comments_enu);
            CommentsTable.Add(LanguageStates.English, EnglishComments);

            string FrenchName = Language.ReplaceHtml(name_fra);
            NameTable.Add(LanguageStates.French, FrenchName);

            string FrenchStatus = Language.ReplaceHtml(status_fra);
            StatusTable.Add(LanguageStates.French, FrenchStatus);

            string FrenchComments = Language.ReplaceHtml(comments_fra);
            CommentsTable.Add(LanguageStates.French, FrenchComments);
        }

        public int Id { get; private set; }
        public int StatusId { get; private set; }
        public string Name { get { return NameTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> NameTable { get; } = new Dictionary<LanguageStates, string>();
        public string Status { get { return StatusTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> StatusTable { get; } = new Dictionary<LanguageStates, string>();
        public string Comments { get { return CommentsTable[LanguageState]; } }
        public Dictionary<LanguageStates, string> CommentsTable { get; } = new Dictionary<LanguageStates, string>();
        public string FriendlyText { get { return Comments.Length > 0 ? $"{Name} ({Comments})" : Name; } }

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
