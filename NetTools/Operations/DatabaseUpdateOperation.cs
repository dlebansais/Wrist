﻿using System;
using System.Collections.Generic;

namespace NetTools
{
    public class DatabaseUpdateOperation : DatabaseOperation
    {
        public DatabaseUpdateOperation(string name, string scriptName, Dictionary<string, string> parameters, Action<int, object> callback)
            : base(name, scriptName, parameters, callback)
        {
        }

        public override string TypeName { get { return "Update"; } }
    }
}
