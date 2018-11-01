using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class NewsEntryLink : ObjectBase, INewsEntryLink
    {
        public NewsEntryLink(int index, string text, string link)
        {
            Index = index;
            Text = $"[{Index}] " + text;
            Link = link;
        }

        public int Index { get; }
        public string Text { get; }
        public string Link { get; }

        public void On_Click(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            destinationPageName = PageNames.accountPage;
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
