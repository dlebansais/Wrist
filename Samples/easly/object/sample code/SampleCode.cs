using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class SampleCode : ObjectBase, ISampleCode
    {
        public SampleCode()
        {
            TitleTable.Add(LanguageStates.English, null);
            TitleTable.Add(LanguageStates.French, null);
        }

        public bool IsFrontPage { get; private set; }
        public string Content { get { return _Content; } }
        private string _Content = "<p>Content</p>";
        public string Feature { get; private set; }
        protected LanguageStates LanguageState { get { return GetLanguage.LanguageState; } }
        public string Title { get { return TitleTable[LanguageState]; } }

        public void UpdateContent(bool isFrontPage, string feature, string content, string titleEnu, string titleFra)
        {
            IsFrontPage = isFrontPage;
            _Content = content;
            Feature = feature;

            TitleTable[LanguageStates.English] = titleEnu;
            TitleTable[LanguageStates.French] = titleFra;

            NotifyPropertyChanged(nameof(IsFrontPage));
            NotifyPropertyChanged(nameof(Content));
            NotifyPropertyChanged(nameof(Feature));
            NotifyPropertyChanged(nameof(Title));
        }

        public Dictionary<LanguageStates, string> TitleTable { get; } = new Dictionary<LanguageStates, string>();

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
