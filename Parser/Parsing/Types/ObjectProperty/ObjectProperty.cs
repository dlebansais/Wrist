namespace Parser
{
    public abstract class ObjectProperty : IObjectProperty
    {
        public ObjectProperty(IDeclarationSource nameSource, string cSharpName, bool isRead, bool isWrite)
        {
            NameSource = nameSource;
            CSharpName = cSharpName;
            IsRead = isRead;
            IsWrite = isWrite;
        }

        public IDeclarationSource NameSource { get; private set; }
        public string CSharpName { get; private set; }
        public bool IsRead { get; private set; }
        public bool IsWrite { get; private set; }

        public void SetIsRead()
        {
            IsRead = true;
        }

        public void SetIsWrite()
        {
            IsWrite = true;
        }

        public void SetIsReadWrite()
        {
            IsRead = true;
            IsWrite = true;
        }

        public abstract bool Connect(IDomain domain);

        public override string ToString()
        {
            string Access = (IsRead ? "R" : "") + (IsWrite ? "W" : "");
            return $"{GetType().Name} '{NameSource.Name}' {Access}";
        }
    }
}
