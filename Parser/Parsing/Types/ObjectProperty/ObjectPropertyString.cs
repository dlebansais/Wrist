namespace Parser
{
    public class ObjectPropertyString : ObjectProperty, IObjectPropertyString
    {
        public ObjectPropertyString(IDeclarationSource nameSource, string cSharpName, int maximumLength)
            : base(nameSource, cSharpName, false, false)
        {
            MaximumLength = maximumLength;
        }

        public int MaximumLength { get; private set; }

        public override bool Connect(IDomain domain)
        {
            return false;
        }

        public override string ToString()
        {
            return base.ToString() +
                ((MaximumLength != int.MaxValue) ? (", Maximum Length=" + MaximumLength.ToString()) : "");
        }
    }
}
