namespace Parser
{
    public class ObjectPropertyReadonlyString : ObjectProperty, IObjectPropertyReadonlyString
    {
        public ObjectPropertyReadonlyString(IDeclarationSource nameSource, string cSharpName, bool isEncrypted)
            : base(nameSource, cSharpName, isEncrypted)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
