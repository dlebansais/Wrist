using System.Collections.Generic;

namespace Parser
{
    public class Empty : LayoutElement, IEmpty
    {
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
