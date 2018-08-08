using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Presentation
{
    public static class Persistent
    {
        #region Persistent State
        private static readonly string PersistentStatePath = "persistent.txt";

        public static string GetValue(string key, string defaultValue)
        {
            if (StateTable == null)
                LoadPersistentState();

            string Result;
            if (StateTable.ContainsKey(key) && !string.IsNullOrEmpty(StateTable[key]))
                Result = StateTable[key];
            else
                Result = defaultValue;

            Debug.WriteLine($"GetValue(\"{key}\") <- {Result}");
            return Result;
        }

        public static void SetValue(string key, string value)
        {
            if (StateTable == null)
                LoadPersistentState();

            if (StateTable.ContainsKey(key))
            {
                Debug.WriteLine($"SetValue(\"{key}\") -> {value}");
                StateTable[key] = value;
            }
            else
            {
                Debug.WriteLine($"[{key}, {value}] added");
                StateTable.Add(key, value);
            }

            Commit();
        }

        private static void LoadPersistentState()
        {
            StateTable = new Dictionary<string, string>();

            if (!FileTools.FileExists(PersistentStatePath))
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

        private static Dictionary<string, string> StateTable = null;
        #endregion
    }
}
