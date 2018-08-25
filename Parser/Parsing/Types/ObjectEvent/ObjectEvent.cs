namespace Parser
{
    public class ObjectEvent : IObjectEvent
    {
        public ObjectEvent(IDeclarationSource nameSource, string cSharpName)
        {
            NameSource = nameSource;
            CSharpName = cSharpName;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public bool? IsProvidingCustomPageName { get; private set; }
        public bool IsUsed { get; private set; }

        public void SetIsProvidingCustomPageName(IDeclarationSource componentSource, bool isSet)
        {
            if (!IsProvidingCustomPageName.HasValue)
                IsProvidingCustomPageName = isSet;
            else if (IsProvidingCustomPageName.Value != isSet)
                throw new ParsingException(173, componentSource.Source, "Incompatible use of event.");
        }

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public void SetIsUsed()
        {
            IsUsed = true;
        }

        public override string ToString()
        {
            return $"{NameSource.Name} event";
        }
    }
}
