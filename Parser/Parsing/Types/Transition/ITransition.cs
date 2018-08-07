using System.Collections.Generic;

namespace Parser
{
    public interface ITransition
    {
        string FromState { get; }
        string ToState { get; }
        IReadOnlyCollection<IObjectProperty> Provides { get; }
        IReadOnlyCollection<IObjectProperty> Unassigns { get; }
    }
}
