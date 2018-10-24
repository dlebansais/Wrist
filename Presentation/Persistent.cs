using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Presentation
{
    public static class Persistent
    {
        public static bool Refresh = false;

        #region Persistent State
        private static readonly string PersistentStatePath = "17661044A27741D982897CF15F6EC1C3.txt";

        public static bool DebugTrace { get; set; } = false;

        public static string GetValue(string key, string defaultValue)
        {
            if (StateTable == null)
                LoadPersistentState();

            string Result;
            if (StateTable.ContainsKey(key) && !string.IsNullOrEmpty(StateTable[key]))
                Result = StateTable[key];
            else
                Result = defaultValue;

            if (DebugTrace)
                Debug.WriteLine($"GetValue(\"{key}\") <- {Result}");

            return Result;
        }

        public static void SetValue(string key, string value)
        {
            if (StateTable == null)
                LoadPersistentState();

            if (StateTable.ContainsKey(key))
            {
                if (DebugTrace)
                    Debug.WriteLine($"SetValue(\"{key}\") -> {value}");

                StateTable[key] = value;
            }
            else
            {
                if (DebugTrace)
                    Debug.WriteLine($"[{key}, {value}] added");

                StateTable.Add(key, value);
            }

            Commit();
        }

        private static void LoadPersistentState()
        {
            StateTable = new Dictionary<string, string>();

            if (Refresh || !FileTools.FileExists(PersistentStatePath))
                FileTools.CommitTextFile(PersistentStatePath, "");

            string PersistentState = FileTools.LoadTextFile(PersistentStatePath, FileMode.Open);

            ParsePersistentState(PersistentState);
        }

        private static void ParsePersistentState(string content)
        {
            string[] Lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string Line in Lines)
            {
                string[] Splitted = Line.Split('=');
                if (Splitted.Length < 2)
                    continue;

                string Key = Splitted[0].Trim().ToLower();
                string Value = Splitted[1];
                for (int i = 2; i < Splitted.Length; i++)
                    Value += "=" + Splitted[i];
                Value = Value.Trim();

                StateTable.Add(Key, Value);

                if (DebugTrace)
                    Debug.WriteLine($"{Key}={Value}");
            }
        }

        private static void Commit()
        {
            string Content = CommitPersistentState();
            FileTools.CommitTextFile(PersistentStatePath, Content);
        }

        private static string CommitPersistentState()
        {
            string Content = "";

            foreach (KeyValuePair<string, string> Entry in StateTable)
                Content += $"{Entry.Key}={Entry.Value}\r\n";

            return Content;
        }

        private static IDictionary<string, string> StateTable = null;
        #endregion
    }
}
