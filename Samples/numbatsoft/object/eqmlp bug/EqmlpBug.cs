using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class EqmlpBug : ObjectBase, IEqmlpBug
    {
        public EqmlpBug(int issue, string appeared, string severity, string version, string description, string analysis, string fix, string binary_path, string readme_path)
        {
            Issue = issue;

            if (appeared == "105")
                Appeared = "Release";
            else
                Appeared = appeared;

            IsFixed = (version != "0");
            IsRegistered = false;

            if (IsFixed)
            {
                BinaryPath = $"{version}/{binary_path}";
                ReadmePath = $"{version}/{readme_path}";
                Fixed = version;
            }
            else
            {
                BinaryPath = "";
                ReadmePath = "";
                Fixed = "";
            }

            Severity = severity;
            Description = string.IsNullOrEmpty(description) ? "" : $"Description: {description}";
            Analysis = string.IsNullOrEmpty(analysis) ? "" : $"Analysis: {analysis}";
            Fix = string.IsNullOrEmpty(fix) ? "" : $"Fix: {fix}";
        }

        public int Issue { get; private set; }
        public string Appeared { get; private set; }
        public string Severity { get; private set; }
        public string Fixed { get; private set; }
        public string Description { get; private set; }
        public string Analysis { get; private set; }
        public bool IsFixed { get; private set; }
        public bool IsRegistered { get; private set; }
        public string Fix { get; private set; }
        public string BinaryPath { get; private set; }
        public string ReadmePath { get; private set; }

        public string BinaryLink
        {
            get
            {
                string BaseUrl = NetTools.UrlTools.GetBaseUrl();
                return $"{BaseUrl}/download/eqmlp/patches/{BinaryPath}";
            }
        }

        public string ReadmeLink
        {
            get
            {
                string BaseUrl = NetTools.UrlTools.GetBaseUrl();
                return $"{BaseUrl}/download/eqmlp/patches/{ReadmePath}";
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
