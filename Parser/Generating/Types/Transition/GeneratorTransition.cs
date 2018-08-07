using System.Collections.Generic;

namespace Parser
{
    public class GeneratorTransition : IGeneratorTransition
    {
        public GeneratorTransition(ITransition transition)
        {
            FromState = transition.FromState;
            ToState = transition.ToState;
            BaseTransition = transition;
            ProvideList = new List<IGeneratorObjectProperty>();
            Provides = ProvideList.AsReadOnly();
            UnassignList = new List<IGeneratorObjectProperty>();
            Unassigns = UnassignList.AsReadOnly();
        }

        private ITransition BaseTransition;
        public string FromState { get; private set; }
        public string ToState { get; private set; }
        public IReadOnlyList<IGeneratorObjectProperty> Provides { get; private set; }
        private List<IGeneratorObjectProperty> ProvideList;
        public IReadOnlyList<IGeneratorObjectProperty> Unassigns { get; private set; }
        private List<IGeneratorObjectProperty> UnassignList;

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (Provides.Count < BaseTransition.Provides.Count || Unassigns.Count < BaseTransition.Unassigns.Count)
            {
                IsConnected = true;

                foreach (IObjectProperty Property in BaseTransition.Provides)
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(Property))
                        if (!ProvideList.Contains(GeneratorObjectProperty.GeneratorObjectPropertyMap[Property]))
                            ProvideList.Add(GeneratorObjectProperty.GeneratorObjectPropertyMap[Property]);

                foreach (IObjectProperty Property in BaseTransition.Unassigns)
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(Property))
                        if (!UnassignList.Contains(GeneratorObjectProperty.GeneratorObjectPropertyMap[Property]))
                            UnassignList.Add(GeneratorObjectProperty.GeneratorObjectPropertyMap[Property]);
            }

            return IsConnected;
        }

        public override string ToString()
        {
            return "From " + FromState.ToString() + " to " + ToState.ToString() +
                ((Provides.Count > 0) ? (", provide " + Provides.Count.ToString()) : "") +
                ((Unassigns.Count > 0) ? (", unassign " + Unassigns.Count.ToString()) : "");
        }
    }
}
