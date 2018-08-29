﻿using System;
using System.Collections.Generic;

namespace DatabaseManager
{
    public class OperationHandler
    {
        public OperationHandler(string handlerName, Func<Dictionary<string, string>, List<Dictionary<string, string>>> handler)
        {
            HandlerName = handlerName;
            Handler = handler;
        }

        public string HandlerName { get; private set; }
        public Func<Dictionary<string, string>, List<Dictionary<string, string>>> Handler { get; private set; }

        private static Dictionary<string, OperationHandler> HandlerTable = new Dictionary<string, OperationHandler>();

        public static void Add(OperationHandler operationHandler)
        {
            HandlerTable.Add(operationHandler.HandlerName, operationHandler);
        }

        public static string Execute(string uriString)
        {
            string HandlerName;
            Dictionary<string, string> Parameters;
            DecomposeUri(uriString, out HandlerName, out Parameters);

            List<Dictionary<string, string>> Result;

            if (HandlerTable.ContainsKey(HandlerName))
                Result = HandlerTable[HandlerName].Handler(Parameters);
            else
                Result = new List<Dictionary<string, string>>();

            return ComposeResult(Result);
        }

        private static void DecomposeUri(string uriString, out string handlerName, out Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>();

            int StartIndex = uriString.IndexOf('?');
            if (StartIndex <=0)
                handlerName = uriString;
            else
            {
                handlerName = uriString.Substring(0, StartIndex);
                StartIndex++;

                while (StartIndex < uriString.Length)
                {
                    string Parameter;

                    int NextIndex = uriString.IndexOf('&', StartIndex);
                    if (NextIndex <= StartIndex)
                    {
                        Parameter = uriString.Substring(StartIndex);
                        StartIndex = uriString.Length;
                    }
                    else
                    {
                        Parameter = uriString.Substring(StartIndex, NextIndex - StartIndex);
                        StartIndex = NextIndex + 1;
                    }

                    string[] Splitted = Parameter.Split('=');
                    if (Splitted.Length >= 2)
                    {
                        string Key = Splitted[0];
                        string Value = Splitted[1];
                        for (int i = 2; i < Splitted.Length; i++)
                            Value += "=" + Splitted[i];

                        parameters.Add(Key, Value);
                    }
                }
            }
        }

        private static string ComposeResult(List<Dictionary<string, string>> data)
        {
            string Result = "";

            foreach (Dictionary<string, string> Line in data)
            {
                Result += Database.RecordPattern + "\n";
                foreach (KeyValuePair<string, string> Entry in Line)
                {
                    Result += Database.StartLinePattern + Entry.Key + "=" + Entry.Value + Database.EndLinePattern + "\n";
                }
            }

            return Result;
        }
    }
}
