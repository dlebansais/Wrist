using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class SampleCode : ObjectBase, ISampleCode
    {
        public SampleCode()
        {
        }

        public bool IsFrontPage { get; private set; }
        public string Content { get; private set; }
        public string Feature { get; private set; }
        public string TitleEnu { get; private set; }
        public string TitleFra { get; private set; }

        public void UpdateContent(bool isFrontPage, string content, string feature, string titleEnu, string titleFra)
        {
            IsFrontPage = isFrontPage;
            Content = content;
            Feature = feature;
            TitleEnu = titleEnu;
            TitleFra = titleFra;

            NotifyPropertyChanged(nameof(IsFrontPage));
            NotifyPropertyChanged(nameof(Content));
            NotifyPropertyChanged(nameof(Feature));
            NotifyPropertyChanged(nameof(TitleEnu));
            NotifyPropertyChanged(nameof(TitleFra));
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
