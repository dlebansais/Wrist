using NetTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppCSHtml5
{
    public class Statuses : ObjectBase, IStatuses
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Statuses()
        {
            InitSimulation();
        }

        public ObservableCollection<IStatusEntry> AllStatus
        {
            get
            {
                GetAllStatus();
                return _AllStatus;
            }
        }
        private ObservableCollection<IStatusEntry> _AllStatus = new ObservableCollection<IStatusEntry>();

        private bool IsAllStatusParsed;

        private void GetAllStatus()
        {
            if (IsAllStatusParsed)
                return;

            IsAllStatusParsed = true;
            GetAllStatus(OnStatusReceived);
        }

        private void OnStatusReceived(int error, object result)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                List<IDictionary<string, string>> StatusList = (List<IDictionary<string, string>>)result;

                foreach (IDictionary<string, string> Item in StatusList)
                {
                    StatusEntry NewEntry = new StatusEntry(Item["id"], Item["level"], Item["label_enu"], Item["detail_enu"], Item["label_fra"], Item["detail_fra"]);
                    _AllStatus.Add(NewEntry);
                }
            }
        }

        #region Operations
        private void GetAllStatus(Action<int, object> callback)
        {
            Database.Query(new DatabaseQueryOperation("get all status entries", "query_all_status.php", new Dictionary<string, string>(), (object sender, CompletionEventArgs e) => OnGetAllStatusCompleted(sender, e, callback)));
        }

        private void OnGetAllStatusCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            List<IDictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "id", "level", "label_enu", "detail_enu", "label_fra", "detail_fra" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler("request/query_all_status.php", OnQueryStatus));
        }

        private List<IDictionary<string, string>> OnQueryStatus(IDictionary<string, string> parameters)
        {
            List<IDictionary<string, string>> Result = new List<IDictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "id", "1" },
                { "level", "1" },
                { "label_enu", "Unimplemented" },
                { "detail_enu", "" },
                { "label_fra", "Non implémenté" },
                { "detail_fra", "" },
            });

            Result.Add(new Dictionary<string, string>()
            {
                { "id", "3" },
                { "level", "30" },
                { "label_enu", "Alpha" },
                { "detail_enu", "partially implemented" },
                { "label_fra", "Alpha" },
                { "detail_fra", "partiellement implémenté" },
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
