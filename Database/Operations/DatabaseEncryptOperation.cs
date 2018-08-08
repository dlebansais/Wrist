using System;
using System.Collections.Generic;

namespace Database
{
    public class DatabaseEncryptOperation : DatabaseOperation
    {
        public DatabaseEncryptOperation(string name, string scriptName, string parameterName, string parameterValue, Action<bool, object> callback)
            : base(name, scriptName, new Dictionary<string, string>() { { parameterName, parameterValue } }, callback)
        {
        }

        public override string TypeName { get { return "Encrypt"; } }
    }
}
