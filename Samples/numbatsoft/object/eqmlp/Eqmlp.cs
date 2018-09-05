using NetTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Eqmlp : IEqmlp
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Eqmlp()
        {
            InitSimulation();
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }
        public INews GetNews { get { return App.GetNews; } }

        public ObservableCollection<IEqmlpReleaseNote> AllReleases
        {
            get
            {
                GetReleases();
                return _AllReleases;
            }
        }
        private ObservableCollection<IEqmlpReleaseNote> _AllReleases = new ObservableCollection<IEqmlpReleaseNote>();

        private bool IsAllReleasesParsed;

        private void GetReleases()
        {
            if (IsAllReleasesParsed)
                return;

            IsAllReleasesParsed = true;
            GetReleases(OnReleasesReceived);
        }

        private void OnReleasesReceived(int error, object result)
        {
            if (error != (int)ErrorCodes.Success)
                return;

            List<Dictionary<string, string>> ReleasesList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{ReleasesList.Count} release notes received");

            foreach (Dictionary<string, string> Item in ReleasesList)
            {
                EqmlpReleaseNote NewEntry = new EqmlpReleaseNote(Item["created"], Item["revision"], Item["binary_path"], Item["readme_path"]);
                _AllReleases.Add(NewEntry);
            }

            NotifyPropertyChanged(nameof(AllReleases));
        }

        #region Operations
        private void GetReleases(Action<int, object> callback)
        {
            Database.Completed += OnGetReleasesCompleted;
            Database.Query(new DatabaseQueryOperation("get release notes", "query_4.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetReleasesCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetReleasesCompleted notified");
            Database.Completed -= OnGetReleasesCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "created", "revision", "binary_path", "readme_path" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            OperationHandler.Add(new OperationHandler("/request/query_4.php", OnQueryReleases));
        }

        private List<Dictionary<string, string>> OnQueryReleases(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2011-04-20 17:26:21" },
                { "revision", "107" },
                { "binary_path", "{97F3A0EB-D731-4a82-9248-88CC528938E7}.exe" },
                { "readme_path", "readme.txt" },
            });

            return Result;
        }
        #endregion

        private Database Database = Database.Current;

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
