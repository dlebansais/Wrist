namespace Parser
{
    public abstract class ObjectProperty : IObjectProperty
    {
        public ObjectProperty(IDeclarationSource nameSource, string cSharpName, bool isEncrypted)
        {
            NameSource = nameSource;
            CSharpName = cSharpName;
            IsEncrypted = isEncrypted;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public bool IsEncrypted { get; private set; }

        public abstract bool Connect(IDomain domain);

        public override string ToString()
        {
            string Encrypted = IsEncrypted ? ", Encrypted" : "";
            return $"{GetType().Name} '{NameSource.Name}'{Encrypted}";
        }
    }
}
