using System.Collections.Generic;

namespace Parser
{
    public interface IObject : IForm
    {
        string Name { get; }
        string CSharpName { get; }
        bool IsGlobal { get; }
        IReadOnlyCollection<IObjectProperty> Properties { get; }
        IReadOnlyCollection<IObjectEvent> Events { get; }
    }
}
