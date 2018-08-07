using System.Collections.Generic;

namespace Parser
{
    public interface IGeneratorTransition
    {
        string FromState { get; }
        string ToState { get; }
        IReadOnlyList<IGeneratorObjectProperty> Provides { get; }
        IReadOnlyList<IGeneratorObjectProperty> Unassigns { get; }
    }
}
