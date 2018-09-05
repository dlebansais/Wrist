using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class EqmlpReleaseNote : IEqmlpReleaseNote
    {
        public EqmlpReleaseNote(string created, string revision, string binary_path, string readme_path)
        {
            string EnuDate, FraDate;
            NewsEntry.ParseCreated(created, out EnuDate, out FraDate);
            Created = EnuDate;

            Revision = revision;
            BinaryPath = binary_path;
            ReadmePath = readme_path;
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }
        public INews GetNews { get { return App.GetNews; } }

        public string Created { get; private set; }
        public string Revision { get; private set; }
        public string BinaryPath { get; private set; }
        public string ReadmePath { get; private set; }
        public string ExternalLink
        {
            get
            {
                string BaseUrl = NetTools.UrlTools.GetBaseUrl();
                return $"{BaseUrl}/download/eqmlp/patches/{Revision}/readme.txt";
            }
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
