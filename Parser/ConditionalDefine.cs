using System.Collections.Generic;

namespace Parser
{
    public class ConditionalDefine
    {
        public ConditionalDefine(string name)
        {
            Name = Normalized(name);
        }

        public string Name { get; private set; }

        private static string Normalized(string text)
        {
            return text.Trim().ToUpper();
        }
    }
}
