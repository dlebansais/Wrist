using System;
using System.Collections.Generic;

namespace Database
{
    public class DatabaseUpdateOperation : DatabaseOperation
    {
        public DatabaseUpdateOperation(string name, string scriptName, Dictionary<string, string> parameters, Action<bool, object> callback)
            : base(name, scriptName, parameters, callback)
        {
        }

        public override string TypeName { get { return "Update"; } }
    }
}
