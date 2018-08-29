using System.Collections.Generic;

namespace Parser
{
    public interface IBackground : IForm, IConnectable
    {
        string Name { get; }
        string XamlName { get; }
        List<string> Lines { get; }
    }
}
