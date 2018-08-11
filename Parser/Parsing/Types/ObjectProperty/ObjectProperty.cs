namespace Parser
{
    public abstract class ObjectProperty : IObjectProperty
    {
        public ObjectProperty(IDeclarationSource nameSource, string cSharpName)
        {
            NameSource = nameSource;
            CSharpName = cSharpName;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }

        public abstract bool Connect(IDomain domain);

        public override string ToString()
        {
            return $"{GetType().Name} '{NameSource.Name}'";
        }
    }
}
