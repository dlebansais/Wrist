namespace Parser
{
    public class ObjectPropertyString : ObjectProperty, IObjectPropertyString
    {
        public ObjectPropertyString(IDeclarationSource nameSource, string cSharpName, bool isEncrypted, int maximumLength, ObjectPropertyStringCategory category)
            : base(nameSource, cSharpName, isEncrypted)
        {
            MaximumLength = maximumLength;
            Category = category;
        }

        public int MaximumLength { get; private set; }
        public ObjectPropertyStringCategory Category { get; private set; }

        public override bool Connect(IDomain domain)
        {
            return false;
        }

        public override string ToString()
        {
            return base.ToString() +
                ((MaximumLength != int.MaxValue) ? (", Maximum Length=" + MaximumLength.ToString()) : "") +
                ((Category != ObjectPropertyStringCategory.Normal) ? (", Category=" + Category.ToString()) : "");
        }
    }
}
