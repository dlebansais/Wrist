namespace Parser
{
    public class ObjectPropertyStringDictionary : ObjectProperty, IObjectPropertyStringDictionary
    {
        public ObjectPropertyStringDictionary(IDeclarationSource nameSource, string cSharpName)
            : base(nameSource, cSharpName, false)
        {
        }

        public override bool Connect(IDomain domain)
        {
            return false;
        }
    }
}
