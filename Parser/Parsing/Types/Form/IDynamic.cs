using System.Collections.Generic;

namespace Parser
{
    public interface IDynamic : IForm
    {
        string Name { get; }
        string FileName { get; }
        string XamlPageName { get; }
        IReadOnlyCollection<IDynamicProperty> Properties { get; }
    }
}
