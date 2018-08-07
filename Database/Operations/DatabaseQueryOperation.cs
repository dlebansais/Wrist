using System;
using System.Diagnostics;

namespace Database
{
    public class DatabaseQueryOperation : DatabaseOperation
    {
        public DatabaseQueryOperation(string name, string schemaName, string queryText, Action<bool, object> callback)
            : base(name, callback)
        {
            SchemaName = schemaName;
            QueryText = queryText;
        }

        public string SchemaName { get; private set; }
        public string QueryText { get; private set; }

        public override void DebugStart()
        {
            Debug.WriteLine($"Get {Name}, sn={SchemaName}, qt={QueryText}");
        }
    }
}
