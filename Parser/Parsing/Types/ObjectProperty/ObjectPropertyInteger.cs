namespace Parser
{
    public class ObjectPropertyInteger : ObjectPropertyIndex, IObjectPropertyInteger
    {
        public ObjectPropertyInteger(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
