using NetTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        public ISampleCode SampleCode { get { return null; } }

        public ObservableCollection<ISampleCode> AllSamples
        {
            get
            {
                GetAllSamples();
                return _AllSamples;
            }
        }
        private ObservableCollection<ISampleCode> _AllSamples = new ObservableCollection<ISampleCode>();

        private bool IsAllSamplesParsed;

        private void GetAllSamples()
        {
            if (IsAllSamplesParsed)
                return;

            IsAllSamplesParsed = true;
            GetAllSamples(OnSampleCodesReceived);
        }

        private void OnSampleCodesReceived(int error, object result)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                List<Dictionary<string, string>> NewsList = (List<Dictionary<string, string>>)result;

                foreach (Dictionary<string, string> Item in NewsList)
                {
                    ISampleCode NewSampleCode = new SampleCode("");
                    _AllSamples.Add(NewSampleCode);
                }
            }
        }

        #region Operations
        private void GetAllSamples(Action<int, object> callback)
        {
            Database.Completed += OnGetAllSampleCodesCompleted;
            Database.Query(new DatabaseQueryOperation("get all sample codes", "query_all_sample_codes.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetAllSampleCodesCompleted(object sender, CompletionEventArgs e)
        {
            Database.Completed -= OnGetAllSampleCodesCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "content" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
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
