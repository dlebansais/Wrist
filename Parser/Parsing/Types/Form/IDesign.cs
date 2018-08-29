using System.Collections.Generic;
using System.Windows;

namespace Parser
{
    public interface IDesign : IForm, IConnectable
    {
        List<string> FileNames { get; }
        string Name { get; }
        string XamlName { get; }
        ResourceDictionary Root { get; }
    }
}
