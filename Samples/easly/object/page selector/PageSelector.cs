using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public class PageSelector : ObjectBase, IPageSelector
    {
        public PageSelector()
        {
        }

        public bool IsHome { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "homePage"; } }
        public bool IsNews { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "newsPage"; } }
        public bool IsDocumentation { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "documentationPage"; } }
        public bool IsDownload { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "downloadPage"; } }
        public bool IsExamples { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "examplesPage"; } }
        public bool IsContact { get { return (Window.Current.Content is Page CurrentPage) && CurrentPage.GetType().Name == "contactPage"; } }

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
