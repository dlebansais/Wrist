using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Browser;
using Windows.UI.Xaml;

namespace AppCSHtml5
{
    public class Download : ObjectBase, IDownload
    {
        public Download()
        {
        }

        public void On_DownloadPg(PageNames pageName, string sourceName, string sourceContent)
        {
            StartDownload("download/pg/pgjsonp.zip");
        }

        public void On_DownloadEqmlp(PageNames pageName, string sourceName, string sourceContent)
        {
            StartDownload("download/eqmlp/install.exe");
        }

        public void On_DownloadUacpt(PageNames pageName, string sourceName, string sourceContent)
        {
            StartDownload("download/uacpt/uacpt.exe");
        }

        public void On_DownloadCcs(PageNames pageName, string sourceName, string sourceContent)
        {
            StartDownload("download/ccs/suite.zip");
        }

        private void StartDownload(string relativeUrl)
        {
            string BaseUrl = NetTools.UrlTools.GetBaseUrl();
            string FullUrl = $"{BaseUrl}/{relativeUrl}";

            DispatcherTimer DownloadTimer = new DispatcherTimer();
            DownloadTimer.Interval = TimeSpan.FromSeconds(3);
            DownloadTimer.Tick += (object sender, object e) => OnDownloadBegin(sender, e, FullUrl);
            DownloadTimer.Start();
        }

        private void OnDownloadBegin(object sender, object e, string url)
        {
            DispatcherTimer DownloadTimer = (DispatcherTimer)sender;
            DownloadTimer.Stop();

            //System.Diagnostics.Debug.WriteLine($"Attempting to download {url}");

            HtmlPage.Window.Navigate(new System.Uri(url, System.UriKind.RelativeOrAbsolute));
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
