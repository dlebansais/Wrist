using System.Collections.Generic;

namespace Parser
{
    public interface IDynamic : IForm
    {
        string Name { get; }
        string CSharpName { get; }
        IReadOnlyCollection<IDynamicProperty> Properties { get; }
    }
}
