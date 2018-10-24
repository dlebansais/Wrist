﻿using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

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

            Database.DebugLog = true;
            Database.DebugLogFullResponse = true;
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
                Dictionary<string, string> Item = (Dictionary<string, string>)result;

                SampleCode SampleCode = _AllSampleCodes[title];
                SampleCode.UpdateContent(Item["front_page"] != "0", Item["feature"], Encoding.UTF8.GetString(Convert.FromBase64String(Item["text"])), Item["title_enu"], Item["title_fra"]);
            }
        }

        #region Transactions
        private void GetSampleCode(string title, Action<int, object> callback)
        {
            Database.Query(new DatabaseQueryOperation("get sample code", "query_sample_code.php", new Dictionary<string, string>() { { "title", HtmlString.PercentEncoded(title) } }, (object sender, CompletionEventArgs e) => OnGetAllSampleCodesCompleted(sender, e, callback)));
        }

        private void OnGetAllSampleCodesCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            IDictionary<string, string> Result;
            if ((Result = Database.ProcessSingleResponse(e.Operation, new List<string>() { "front_page", "feature", "text", "title_enu", "title_fra", "result" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback(ParseResult(Result["result"]), Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.AnyError, null));
        }

        public static int ParseResult(string result)
        {
            int IntError;
            if (int.TryParse(result, out IntError))
                return IntError;
            else
                return -1;
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler($"{Database.QueryScriptPath}query_sample_code.php", OnQuerySampleCode));
        }

        private List<Dictionary<string, string>> OnQuerySampleCode(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "front_page", "0" },
                { "feature", "abstract_methods" },
                { "text", "<p><span id=\"sc_neutral\">Insert</span>&nbsp;<span id=\"sc_keyword\">procedure</span></p>" },
                { "title_enu", "Declaration of an abstract method" },
                { "title_fra", "Déclaration d'une méthode abstraite" },
                { "result", ((int)ErrorCodes.Success).ToString() },
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
