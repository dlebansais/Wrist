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
    public class Features : ObjectBase, IFeatures
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Features()
        {
            InitSimulation();
        }

        public ObservableCollection<IFeatureEntry> AllFeatures
        {
            get
            {
                GetAllFeatures();
                return _AllFeatures;
            }
        }
        private ObservableCollection<IFeatureEntry> _AllFeatures = new ObservableCollection<IFeatureEntry>();

        private bool IsAllFeaturesParsed;

        private void GetAllFeatures()
        {
            if (IsAllFeaturesParsed)
                return;

            IsAllFeaturesParsed = true;
            GetAllFeatures(OnFeatureReceived);
        }

        private void OnFeatureReceived(int error, object result)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                List<IDictionary<string, string>> FeatureList = (List<IDictionary<string, string>>)result;

                foreach (IDictionary<string, string> Item in FeatureList)
                {
                    FeatureEntry NewEntry = new FeatureEntry(Item["id"], Item["status_id"], Item["name_enu"], Item["label_enu"], Item["comments_enu"], Item["name_fra"], Item["label_fra"], Item["comments_fra"]);
                    _AllFeatures.Add(NewEntry);
                }
            }
        }

        #region Operations
        private void GetAllFeatures(Action<int, object> callback)
        {
            Database.Query(new DatabaseQueryOperation("get all feature entries", "features/query_all_features.php", new Dictionary<string, string>(), (object sender, CompletionEventArgs e) => OnGetAllFeaturesCompleted(sender, e, callback)));
        }

        private void OnGetAllFeaturesCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            List<IDictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "id", "status_id", "name_enu", "label_enu", "comments_enu", "name_fra", "label_fra", "comments_fra" })) != null)
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

            OperationHandler.Add(new OperationHandler($"/{Database.QueryScriptPath}features/query_all_features.php", OnQueryFeature));
        }

        private List<IDictionary<string, string>> OnQueryFeature(IDictionary<string, string> parameters)
        {
            List<IDictionary<string, string>> Result = new List<IDictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "id", "1" },
                { "status_id", "1" },
                { "name_enu", "Abstract methods" },
                { "label_enu", "Unimplemented" },
                { "comments_enu", "" },
                { "name_fra", "Méthodes abstraites" },
                { "label_fra", "Non implémenté" },
                { "comments_fra", "" },
            });

            Result.Add(new Dictionary<string, string>()
            {
                { "id", "2" },
                { "status_id", "3" },
                { "name_enu", "Output to C# code" },
                { "label_enu", "Alpha" },
                { "comments_enu", "Making the output of the compiler a list of C# files that can be used in a Visual Studio project" },
                { "name_fra", "Production de code en C#" },
                { "label_fra", "Alpha" },
                { "comments_fra", "Obtenir du compilateur une liste de fichiers C# qui peuvent être utilisés dans un projet Visual Studio" },
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
