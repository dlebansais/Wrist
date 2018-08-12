namespace Parser
{
    public class ObjectEvent : IObjectEvent
    {
        public ObjectEvent(string name, string cSharpName)
        {
            Name = name;
            CSharpName = cSharpName;
        }

        public string Name { get; private set; }
        public string CSharpName { get; private set; }
        public bool? IsProvidingCustomPageName { get; private set; }

        public void SetIsProvidingCustomPageName(IDeclarationSource componentSource, bool isSet)
        {
            if (!IsProvidingCustomPageName.HasValue)
                IsProvidingCustomPageName = isSet;
            else if (IsProvidingCustomPageName.Value != isSet)
                throw new ParsingException(173, componentSource.Source, "Incompatible use of event.");
        }

        public override string ToString()
        {
            return $"{Name} event";
        }
    }
}
