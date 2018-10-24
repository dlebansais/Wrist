using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppCSHtml5
{
    public class Feedback : ObjectBase, IFeedback
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
        }

        public Feedback()
        {
        }

        public string Content { get; set; }

        public void On_Send(PageNames pageName, string sourceName, string sourceContent)
        {
            string ContentToSend = Convert.ToBase64String(Encoding.UTF8.GetBytes(Content));
            Content = null;

            SendFeedback(ContentToSend, (int errorCode, object result) => { });
        }

        #region Operations
        private void SendFeedback(string content, Action<int, object> callback)
        {
            Database.Update(new DatabaseUpdateOperation("send feedback", "update_feedback.php", new Dictionary<string, string>() { { "content", HtmlString.PercentEncoded(content) } }, (object sender, CompletionEventArgs e) => OnSendFeedbackCompleted(sender, e, callback)));
        }

        private void OnSendFeedbackCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.Success, new Dictionary<string, string>()));
        }

        private Database Database = Database.Current;
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler("request/update_feedback.php", OnCompleteSendFeedback));
        }

        private List<IDictionary<string, string>> OnCompleteSendFeedback(IDictionary<string, string> parameters)
        {
            List<IDictionary<string, string>> Result = new List<IDictionary<string, string>>();

            string Content;
            if (parameters.ContainsKey("content"))
                Content = parameters["content"];
            else
                Content = null;

            return Result;
        }
        #endregion

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
