namespace Parser
{
    public class ObjectPropertyReadonlyString : ObjectProperty, IObjectPropertyReadonlyString
    {
        public ObjectPropertyReadonlyString(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
