using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Error : ObjectBase, IError
    {
        public void On_Error404(PageNames pageName, IObjectBase senderContext, string sourceName, string sourceContent, out PageNames destinationPageName)
        {
            IDictionary<string, string> QueryString = App.QueryString;
            if (QueryString != null && QueryString.ContainsKey("error") && QueryString["error"] == "404")
                destinationPageName = PageNames.error_404Page;
            else
                destinationPageName = pageName;
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
