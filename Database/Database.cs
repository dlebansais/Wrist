using System.Collections.Generic;
using System.Diagnostics;
#if HTTP
using System;
using System.Net;
using Windows.UI.Xaml;
#else
#endif

namespace Database
{
    public class Database
    {
        public bool DebugWriteResponse { get; set; } = true;
        public string QueryScriptPath { get; set; } = "/query/query.php";
        public string UpdateScriptPath { get; set; } = "/query/update.php";
        public string EncryptScriptPath { get; set; } = "/query/encrypt.php";

        public event CompletionEventHandler Completed;
        public Dictionary<DatabaseOperation, List<Dictionary<string, object>>> RequestResultTable { get; } = new Dictionary<DatabaseOperation, List<Dictionary<string, object>>>();

#if HTTP
        public void Query(DatabaseQueryOperation operation)
        {
            operation.DebugStart();

            string AddressString = $"{QueryScriptPath}?sn={operation.SchemaName}&qt={operation.QueryText}";
            if (!DownloadClientTable.ContainsKey(AddressString))
            {
                DownloadClientTable.Add(AddressString, new KeyValuePair<DatabaseOperation, WebClient>(operation, new WebClient()));
                Debug.WriteLine("Get added");

                PopRequest();
            }
            else
                Debug.WriteLine("An identical get request is already queued");
        }

        public void Update(DatabaseUpdateOperation operation)
        {
            operation.DebugStart();

            string AddressString = $"{UpdateScriptPath}?sn={operation.SchemaName}&ut={operation.UpdateText}";
            if (!DownloadClientTable.ContainsKey(AddressString))
            {
                DownloadClientTable.Add(AddressString, new KeyValuePair<DatabaseOperation, WebClient>(operation, new WebClient()));
                Debug.WriteLine("Set added");

                PopRequest();
            }
            else
                Debug.WriteLine("An identical update request is already queued");
        }

        public void Encrypt(DatabaseEncryptOperation operation)
        {
            operation.DebugStart();

            string AddressString = $"{EncryptScriptPath}?pt={operation.PlainText}";
            if (!DownloadClientTable.ContainsKey(AddressString))
            {
                DownloadClientTable.Add(AddressString, new KeyValuePair<DatabaseOperation, WebClient>(operation, new WebClient()));
                Debug.WriteLine("Encrypt added");

                PopRequest();
            }
            else
                Debug.WriteLine("An identical encrypt request is already queued");
        }

        private void PopRequest()
        {
            try
            {
                foreach (KeyValuePair<string, KeyValuePair<DatabaseOperation, WebClient>> Entry in DownloadClientTable)
                {
                    string AddressString = Entry.Key;
                    DatabaseOperation Operation = Entry.Value.Key;
                    WebClient DownloadClient = Entry.Value.Value;

                    Debug.WriteLine("Request started");

                    CurrentDownload = DownloadClient;
                    CurrentOperation = Operation;

                    Uri UriAddress = new Uri(AddressString, UriKind.Relative);
                    DownloadClient.DownloadStringCompleted += OnDownloadCompleted;
                    DownloadClient.DownloadStringAsync(UriAddress);

                    Debug.WriteLine("Download async started");
                    return;
                }

                Debug.WriteLine("No more request");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void OnDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Debug.WriteLine("Download async completed");

            WebClient DownloadClient = CurrentDownload;
            DatabaseOperation Operation = CurrentOperation;
            CurrentDownload = null;
            CurrentOperation = null;

            if (DownloadClient != null)
            {
                foreach (KeyValuePair<string, KeyValuePair<DatabaseOperation, WebClient>> Entry in DownloadClientTable)
                    if (DownloadClient == Entry.Value.Value)
                    {
                        DownloadClientTable.Remove(Entry.Key);
                        Debug.WriteLine("Request withdrawn");
                        break;
                    }

                Debug.WriteLine($"Request {Operation.Name} completed");

                List<Dictionary<string, object>> RequestResult = new List<Dictionary<string, object>>();

                try
                {
                    string Content = e.Result;
                    RequestResult = ParseResponse(Content);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                RequestResultTable.Add(Operation, RequestResult);
                Debug.WriteLine($"Request {Operation.Name} result available");
                Completed?.Invoke(this, new CompletionEventArgs(Operation));
            }

            PopRequest();
        }

        private Dictionary<string, KeyValuePair<DatabaseOperation, WebClient>> DownloadClientTable = new Dictionary<string, KeyValuePair<DatabaseOperation, WebClient>>();
        private WebClient CurrentDownload = null;
        private DatabaseOperation CurrentOperation = null;
#else
        public void Query(DatabaseQueryOperation operation)
        {
            List<Dictionary<string, object>> Result = new List<Dictionary<string, object>>();
            RequestResultTable.Add(operation, Result);
            Completed?.Invoke(null, new CompletionEventArgs(operation));
        }

        public void Encrypt(DatabaseEncryptOperation operation)
        {
            List<Dictionary<string, object>> Result = new List<Dictionary<string, object>>();
            RequestResultTable.Add(operation, Result);
            Completed?.Invoke(null, new CompletionEventArgs(operation));
        }

        public void Update(DatabaseUpdateOperation operation)
        {
            List<Dictionary<string, object>> Result = new List<Dictionary<string, object>>();
            RequestResultTable.Add(operation, Result);
            Completed?.Invoke(null, new CompletionEventArgs(operation));
        }
#endif

        private List<Dictionary<string, object>> ParseResponse(string content)
        {
            List<Dictionary<string, object>> Result = new List<Dictionary<string, object>>();

            if (DebugWriteResponse)
                Debug.WriteLine(content);

            string RecordPattern = "<p>*</p>";
            string StartLinePattern = "<p>";
            string EndLinePattern = "</p>";

            int RecordStart = 0;
            while ((RecordStart = content.IndexOf(RecordPattern, RecordStart)) >= 0)
            {
                Dictionary<string, object> NewEntry = new Dictionary<string, object>();

                RecordStart += RecordPattern.Length;

                int LineStart = RecordStart;
                while ((LineStart = content.IndexOf(StartLinePattern, LineStart)) >= 0)
                {
                    LineStart += StartLinePattern.Length;
                    int LineEnd = content.IndexOf(EndLinePattern, LineStart);
                    if (LineEnd < 0)
                        LineEnd = content.Length;

                    string Line = content.Substring(LineStart, LineEnd - LineStart);

                    string[] Splitted = Line.Split('=');
                    if (Splitted.Length > 1)
                    {
                        string Key = Splitted[0];
                        string Value = Splitted[1];
                        for (int i = 2; i < Splitted.Length; i++)
                            Value += '=' + Splitted[i];

                        if (!NewEntry.ContainsKey(Key))
                            NewEntry.Add(Key, Value);
                    }
                }

                Result.Add(NewEntry);
            }

            return Result;
        }

        public string HtmlString(string s)
        {
            s = s.Replace(" ", " ");

            return s;
        }

        public void DebugWriteState()
        {
            foreach (KeyValuePair<DatabaseOperation, List<Dictionary<string, object>>> ResultEntry in RequestResultTable)
            {
                DatabaseOperation Operation = ResultEntry.Key;
                List<Dictionary<string, object>> ResultList = ResultEntry.Value;
                Debug.WriteLine($"Operation: {Operation.Name}, {ResultList.Count} entries");

                for (int i = 0; i < ResultList.Count; i++)
                {
                    Dictionary<string, object> Item = ResultList[i];
                    Debug.WriteLine($"#{i}, {Item.Count} keys");

                    foreach (KeyValuePair<string, object> Entry in Item)
                        Debug.WriteLine($"{Entry.Key} -> {Entry.Value}");
                }
            }
        }

        public Dictionary<string, string> ProcessSingleResponse(DatabaseOperation operation, List<string> keyList)
        {
            Dictionary<string, string>  Result = null;

            if (RequestResultTable.ContainsKey(operation))
            {
                List<Dictionary<string, object>> ResultList = RequestResultTable[operation];
                RequestResultTable.Remove(operation);

                foreach (Dictionary<string, object> Item in ResultList)
                {
                    foreach (string Key in keyList)
                    {
                        if (Item.ContainsKey(Key) && (Result == null || !Result.ContainsKey(Key)) && Item[Key] is string)
                        {
                            if (Result == null)
                                Result = new Dictionary<string, string>();

                            Result.Add(Key, Item[Key] as string);
                        }
                    }

                    break;
                }
            }

            return Result;
        }

        public List<Dictionary<string, string>> ProcessMultipleResponse(DatabaseOperation operation, List<string> keyList)
        {
            List<Dictionary<string, string>> Result = null;

            if (RequestResultTable.ContainsKey(operation))
            {
                List<Dictionary<string, object>> ResultList = RequestResultTable[operation];
                RequestResultTable.Remove(operation);

                foreach (Dictionary<string, object> Item in ResultList)
                {
                    Dictionary<string, string> ResultLine = null;
                    foreach (string Key in keyList)
                    {
                        if (Item.ContainsKey(Key) && (ResultLine == null || !ResultLine.ContainsKey(Key)) && Item[Key] is string)
                        {
                            if (ResultLine == null)
                            {
                                ResultLine = new Dictionary<string, string>();
                                if (Result == null)
                                    Result = new List<Dictionary<string, string>>();

                                Result.Add(ResultLine);
                            }

                            ResultLine.Add(Key, Item[Key] as string);
                        }
                    }
                }
            }

            return Result;
        }
    }
}
