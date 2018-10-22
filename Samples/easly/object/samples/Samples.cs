using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Samples : ObjectBase, ISamples
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Samples()
        {
            InitSimulation();
        }

        private Dictionary<string, SampleCode> _AllSampleCodes = new Dictionary<string, SampleCode>();
        public ISampleCode AbstractMethod0 { get { return GetSampleCode("Declaration of an abstract method"); } }
        public ISampleCode AbstractMethod1 { get { return GetSampleCode("Declaration of an abstract class"); } }
        public ISampleCode AbstractMethod2 { get { return GetSampleCode("Method inherited as abstract"); } }

        private ISampleCode GetSampleCode(string title)
        {
            if (!_AllSampleCodes.ContainsKey(title))
            {
                _AllSampleCodes.Add(title, new SampleCode());
                GetSampleCode(title, (int error, object result) => OnSampleCodeReceived(error, result, title));
            }
            return _AllSampleCodes[title];
        }

        private void OnSampleCodeReceived(int error, object result, string title)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                List<Dictionary<string, string>> ResultItems = (List<Dictionary<string, string>>)result;
                foreach (Dictionary<string, string> Item in ResultItems)
                    if (Item.ContainsKey("is_front_page") &&
                        Item.ContainsKey("feature") &&
                        Item.ContainsKey("content") &&
                        Item.ContainsKey("title_enu") &&
                        Item.ContainsKey("title_fra"))
                    {
                        SampleCode SampleCode = _AllSampleCodes[title];
                        SampleCode.UpdateContent(Item["is_front_page"] != "0", Item["feature"], Item["content"], Item["title_enu"], Item["title_fra"]);
                    }
            }
        }

        #region Operations
        private void GetSampleCode(string title, Action<int, object> callback)
        {
            Database.Completed += OnGetAllSampleCodesCompleted;
            Database.Query(new DatabaseQueryOperation("get sample code", "query_sample_code.php", new Dictionary<string, string>() { { "title", HtmlString.PercentEncoded(title) } }, callback));
        }

        private void OnGetAllSampleCodesCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnGetAllSampleCodesCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "is_front_page", "feature", "content", "title_enu", "title_fra" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler("/request/query_sample_code.php", OnQuerySampleCode));
        }

        private List<Dictionary<string, string>> OnQuerySampleCode(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "is_front_page", "0" },
                { "feature", "abstract_methods" },
                { "content", "<p><span id=\"sc_neutral\">Insert</span>&nbsp;<span id=\"sc_keyword\">procedure</span></p>" },
                { "title_enu", "Declaration of an abstract method" },
                { "title_fra", "Déclaration d'une méthode abstraite" },
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
