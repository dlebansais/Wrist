﻿using System.Collections.Generic;
using System.Diagnostics;
using System;
#if USE_RESTRICTED_FEATURES
using System.Net;
#else
#endif

namespace NetTools
{
    public class Database
    {
        public static Database Current = new Database();

        private Database()
        {
        }

        public bool DebugLog { get; set; } = false;
        public bool DebugLogFullResponse { get; set; } = false;
        public string QueryScriptPath { get; set; } = "/request/";
        public string UpdateScriptPath { get; set; } = "/request/";
        public string EncryptScriptPath { get; set; } = "/request/";

        public event CompletionEventHandler Completed;
        public Dictionary<DatabaseOperation, List<Dictionary<string, object>>> RequestResultTable { get; } = new Dictionary<DatabaseOperation, List<Dictionary<string, object>>>();

        public void AddLogEntry(string text)
        {
            Debug.WriteLine(text);
        }

        public void Query(DatabaseQueryOperation operation)
        {
            StartRequest(operation);
        }

        public void Update(DatabaseUpdateOperation operation)
        {
            StartRequest(operation);
        }

        public void Encrypt(DatabaseEncryptOperation operation)
        {
            StartRequest(operation);
        }

        private void StartRequest(DatabaseOperation operation)
        {
            operation.DebugStart();

            string AddressString = operation.RequestString(QueryScriptPath);
            if (!DownloadClientTable.ContainsKey(AddressString))
            {
                DownloadClientTable.Add(AddressString, new KeyValuePair<DatabaseOperation, WebClient>(operation, new WebClient()));
                AddLogEntry($"{operation.TypeName} added");

                PopRequest();
            }
            else
                AddLogEntry($"An identical {operation.TypeName} request is already queued");
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

                    AddLogEntry("Request started");

                    CurrentDownload = DownloadClient;
                    CurrentOperation = Operation;

                    Uri UriAddress = new Uri(AddressString, UriKind.Relative);
                    DownloadClient.DownloadStringCompleted += OnDownloadCompleted;
                    DownloadClient.DownloadStringAsync(UriAddress);

                    AddLogEntry("Download async started");
                    return;
                }

                AddLogEntry("No more request");
            }
            catch (Exception e)
            {
                AddLogEntry(e.Message);
            }
        }

        public void OnDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            AddLogEntry("Download async completed");

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
                        AddLogEntry("Request withdrawn");
                        break;
                    }

                AddLogEntry($"Request {Operation.Name} completed");

                List<Dictionary<string, object>> RequestResult = new List<Dictionary<string, object>>();

                try
                {
                    string Content = e.Result;
                    RequestResult = ParseResponse(Content);
                }
                catch (Exception ex)
                {
                    AddLogEntry(ex.Message);
                }

                RequestResultTable.Add(Operation, RequestResult);
                AddLogEntry($"Request {Operation.Name} result available");
                Completed?.Invoke(this, new CompletionEventArgs(Operation));
            }

            PopRequest();
        }

        private Dictionary<string, KeyValuePair<DatabaseOperation, WebClient>> DownloadClientTable = new Dictionary<string, KeyValuePair<DatabaseOperation, WebClient>>();
        private WebClient CurrentDownload = null;
        private DatabaseOperation CurrentOperation = null;

        public static readonly string RecordPattern = "<p>*</p>";
        public static readonly string StartLinePattern = "<p>";
        public static readonly string EndLinePattern = "</p>";

        private List<Dictionary<string, object>> ParseResponse(string content)
        {
            List<Dictionary<string, object>> Result = new List<Dictionary<string, object>>();

            if (DebugLogFullResponse)
                AddLogEntry(content);

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

        public void DebugWriteState()
        {
            foreach (KeyValuePair<DatabaseOperation, List<Dictionary<string, object>>> ResultEntry in RequestResultTable)
            {
                DatabaseOperation Operation = ResultEntry.Key;
                List<Dictionary<string, object>> ResultList = ResultEntry.Value;
                AddLogEntry($"Operation: {Operation.Name}, {ResultList.Count} entries");

                for (int i = 0; i < ResultList.Count; i++)
                {
                    Dictionary<string, object> Item = ResultList[i];
                    AddLogEntry($"#{i}, {Item.Count} keys");

                    foreach (KeyValuePair<string, object> Entry in Item)
                        AddLogEntry($"{Entry.Key} -> {Entry.Value}");
                }
            }
        }

        public Dictionary<string, string> ProcessSingleResponse(DatabaseOperation operation, List<string> keyList)
        {
            Dictionary<string, string> Result = null;

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
