using System;
using System.Diagnostics;

namespace Database
{
    public class DatabaseUpdateOperation : DatabaseOperation
    {
        public DatabaseUpdateOperation(string name, string schemaName, string updateText, Action<bool, object> callback)
            : base(name, callback)
        {
            SchemaName = schemaName;
            UpdateText = updateText;
        }

        public string SchemaName { get; private set; }
        public string UpdateText { get; private set; }

        public override void DebugStart()
        {
            Debug.WriteLine($"Set {Name}, sn={SchemaName}, ut={UpdateText}");
        }
    }
}
