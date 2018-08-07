using System;

namespace Database
{
    public abstract class DatabaseOperation
    {
        public DatabaseOperation(string name, Action<bool, object> callback)
        {
            Name = name;
            Callback = callback;
        }

        public string Name { get; private set; }
        public Action<bool, object> Callback { get; private set; }

        public abstract void DebugStart();
    }
}
