using System.Collections.Generic;

namespace Parser
{
    public class Transition : ITransition
    {
        public Transition(string fromState, string toState, IObjectPropertyCollection provides, IObjectPropertyCollection unassigns)
        {
            FromState = fromState;
            ToState = toState;
            Provides = provides.AsReadOnly();
            Unassigns = provides.AsReadOnly();
        }

        public string FromState { get; private set; }
        public string ToState { get; private set; }
        public IReadOnlyCollection<IObjectProperty> Provides { get; private set; }
        public IReadOnlyCollection<IObjectProperty> Unassigns { get; private set; }

        public override string ToString()
        {
            return "From " + FromState.ToString() + " to " + ToState.ToString() +
                ((Provides.Count > 0) ? (", provide " + Provides.Count.ToString()) : "") +
                ((Unassigns.Count > 0) ? (", unassign " + Unassigns.Count.ToString()) : "");
        }
    }
}
