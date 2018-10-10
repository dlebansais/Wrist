using System.Collections.Generic;

namespace Parser
{
    public interface IConnectable
    {
        bool Connect(IDomain domain);
    }
}
