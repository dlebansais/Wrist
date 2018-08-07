using System;
using System.Diagnostics;

namespace Database
{
    public class DatabaseEncryptOperation : DatabaseOperation
    {
        public DatabaseEncryptOperation(string name, string plainText, Action<bool, object> callback)
            : base(name, callback)
        {
            PlainText = plainText;
        }

        public string PlainText { get; private set; }

        public override void DebugStart()
        {
            Debug.WriteLine($"Encrypt {Name}, pt={PlainText}");
        }
    }
}
