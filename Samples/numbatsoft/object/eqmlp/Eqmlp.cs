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
    public class Eqmlp : ObjectBase, IEqmlp
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

        public string TutorialLink { get { return $"https://www.enu.numbatsoft.com/products/eqmlp/documentation/xaml_tutorial%20(draft).txt"; } }

        #region Release Notes
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
        #endregion

        #region Bugs
        public ObservableCollection<IEqmlpBug> AllBugs
        {
            get
            {
                GetBugs();
                return _AllBugs;
            }
        }
        private ObservableCollection<IEqmlpBug> _AllBugs = new ObservableCollection<IEqmlpBug>();

        private bool IsAllBugsParsed;

        private void GetBugs()
        {
            if (IsAllBugsParsed)
                return;

            IsAllBugsParsed = true;
            GetBugs(OnBugsReceived);
        }

        private void OnBugsReceived(int error, object result)
        {
            if (error != (int)ErrorCodes.Success)
                return;

            List<Dictionary<string, string>> BugsList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{BugsList.Count} bugs received");

            int issue = 1;
            foreach (Dictionary<string, string> Item in BugsList)
            {
                EqmlpBug NewEntry = new EqmlpBug(issue++, Item["appeared"], Item["severity"], Item["fixed"], Item["description"], Item["analysis"], Item["fix"], Item["binary_path"], Item["readme_path"]);
                _AllBugs.Add(NewEntry);
            }

            NotifyPropertyChanged(nameof(AllBugs));
        }
        #endregion

        #region Organizations
        public ObservableCollection<IEqmlpOrganization> AllOrganizations
        {
            get
            {
                GetOrganizations();
                return _AllOrganizations;
            }
        }
        private ObservableCollection<IEqmlpOrganization> _AllOrganizations = new ObservableCollection<IEqmlpOrganization>();

        private bool IsAllOrganizationsParsed;

        private void GetOrganizations()
        {
            if (IsAllOrganizationsParsed)
                return;

            IsAllOrganizationsParsed = true;
            GetOrganizations(OnOrganizationsReceived);
        }

        private void OnOrganizationsReceived(int error, object result)
        {
            if (error != (int)ErrorCodes.Success)
                return;

            List<Dictionary<string, string>> OrganizationsList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{OrganizationsList.Count} organizations received");

            foreach (Dictionary<string, string> Item in OrganizationsList)
            {
                EqmlpOrganization NewEntry = new EqmlpOrganization(Item["name"], Item["login_url"], Item["meeting_url"], Item["validation_url"]);
                _AllOrganizations.Add(NewEntry);
            }

            NotifyPropertyChanged(nameof(AllOrganizations));
        }
        #endregion

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

        private void GetBugs(Action<int, object> callback)
        {
            Database.Completed += OnGetBugsCompleted;
            Database.Query(new DatabaseQueryOperation("get bugs", "query_5.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetBugsCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetBugsCompleted notified");
            Database.Completed -= OnGetBugsCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "appeared", "severity", "fixed", "description", "analysis", "fix", "binary_path", "readme_path" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }

        private void GetOrganizations(Action<int, object> callback)
        {
            Database.Completed += OnGetOrganizationsCompleted;
            Database.Query(new DatabaseQueryOperation("get organizations", "query_6.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetOrganizationsCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetOrganizationsCompleted notified");
            Database.Completed -= OnGetOrganizationsCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "name", "login_url", "meeting_url", "validation_url" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            OperationHandler.Add(new OperationHandler("/request/query_4.php", OnQueryReleases));
            OperationHandler.Add(new OperationHandler("/request/query_5.php", OnQueryBugs));
            OperationHandler.Add(new OperationHandler("/request/query_6.php", OnQueryOrganizations));
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

        private List<Dictionary<string, string>> OnQueryBugs(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "appeared", "101" },
                { "severity", "Low" },
                { "fixed", "110" },
                { "description", "seen" },
                { "analysis", "checked" },
                { "fix", "fixed" },
                { "binary_path", "{97F3A0EB-D731-4a82-9248-88CC528938E7}.exe" },
                { "readme_path", "readme.txt" },
            });

            Result.Add(new Dictionary<string, string>()
            {
                { "appeared", "107" },
                { "severity", "High" },
                { "fixed", "110" },
                { "description", "seen" },
                { "analysis", "checked" },
                { "fix", "fixed" },
                { "binary_path", "{97F3A0EB-D731-4a82-9248-88CC528938E7}.exe" },
                { "readme_path", "readme.txt" },
            });

            Result.Add(new Dictionary<string, string>()
            {
                { "appeared", "108" },
                { "severity", "High" },
                { "fixed", "0" },
                { "description", "seen" },
                { "analysis", "checked" },
                { "fix", "" },
                { "binary_path", "" },
                { "readme_path", "" },
            });

            return Result;
        }

        private List<Dictionary<string, string>> OnQueryOrganizations(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "name", "Shadows of Doom, a guild on Antonius Bayle" },
                { "login_url", "http://www.sodeq.org/login.php" },
                { "meeting_url", "http://www.sodeq.org/meeting.php" },
                { "validation_url", "http://www.sodeq.org/membercheck.php" },
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
