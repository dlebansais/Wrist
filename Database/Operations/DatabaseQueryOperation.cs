using System;
using System.Collections.Generic;

namespace DatabaseManager
{
    public class DatabaseQueryOperation : DatabaseOperation
    {
        public DatabaseQueryOperation(string name, string scriptName, Dictionary<string, string> parameters, Action<bool, object> callback)
            : base(name, scriptName, parameters, callback)
        {
        }

        public override string TypeName { get { return "Query"; } }
    }
}
