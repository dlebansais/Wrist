using System.Collections.Generic;
using System.Windows;

namespace Parser
{
    public interface IDesign : IForm
    {
        List<string> FileNames { get; }
        string Name { get; }
        string XamlName { get; }
        ResourceDictionary Root { get; }
    }
}
