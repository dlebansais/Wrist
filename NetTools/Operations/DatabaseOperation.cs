using System.Collections.Generic;
using System.Diagnostics;

namespace NetTools
{
    public abstract class DatabaseOperation
    {
        public DatabaseOperation(string name, string scriptName, IDictionary<string, string> parameters, CompletionEventHandler callback)
        {
            Name = name;
            ScriptName = scriptName;
            Parameters = parameters;
            Callback = callback;
        }

        public string Name { get; private set; }
        public string ScriptName { get; private set; }
        public IDictionary<string, string> Parameters { get; private set; }
        public CompletionEventHandler Callback { get; private set; }

        public virtual string RequestString(string requestScriptPath)
        {
            string ParameterString = "";
            foreach (KeyValuePair<string, string> Entry in Parameters)
            {
                if (ParameterString.Length == 0)
                    ParameterString += "?";
                else
                    ParameterString += "&";
                ParameterString += $"{Entry.Key}={Entry.Value}";
            }

            string Request = $"{requestScriptPath}{ScriptName}{ParameterString}";

            return Request;
        }

        public abstract string TypeName { get; }

        public virtual void DebugStart()
        {
            string Line = $"{TypeName} {Name}, script={ScriptName}";

            foreach (KeyValuePair<string, string> Entry in Parameters)
                Line += $", {Entry.Key}={Entry.Value}";

            Debug.WriteLine(Line);
        }
    }
}
